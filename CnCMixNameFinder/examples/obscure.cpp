/*******************************************************************************
 ***                      --  R E D  A L E R T  + +  --                      ***
 *******************************************************************************
 *  Project Name:: RedAlert++
 *
 *          File:: OBSCURE.CPP
 *        Author:: OmniBlade
 *  Contributors:: none
 *       History:: 
 *
 *   Description:: 
 *
 * Function List:: Obfuscate()
 *                 Obscure()
 *-----------------------------------------------------------------------------*/
 
#include	"obscure.h"
#include	"strutil.h"
#include	<string.h>
#include	<ctype.h>

#define		OBSCURE_MAGIC_NUM	0x516150

//------------------------------------------------------------------------------

/*******************************************************************************
 * NAME:         Obfuscate()
 * DESCRIPTION:  <function overview>
 * PARAMETERS:   none
 * RETURN:       none
 * WARNINGS:     none
 *============================================================================*/
template<typename CRC>  //eg. T == Compute_Hash<CRC32Engine>
sint32 Obfuscate(char const *string)
{
    static char _lossbits[8] = { 0, 8, 0, 32, 0, 4, 16, 0 };
    static char _addbits[8] = { 16, 0, 0, -128, 64, 0, 0, 4 };
    
    char buffer[128];

    if ( string != nullptr ) {

        memset(buffer, 0xA5, 128);  //fill data
        strncpy(buffer, string, 128);
        int length = (int)strlen(buffer);
        StrUtil::strupr(buffer);
        
        for ( int i = 0; i < length; ++i ) {

            //test if we have a printable char, if not, set it to i % 26 uppercase
            if ( !isgraph(buffer[i]) ) {
                buffer[i] = i % 26 + 'A';
            }

        }
        
        if ( length < 16 || length & 3 ) {

            int padding = 16;
            
            if ( (((uint8)length + 3) & 252) > 16 ) {
                padding = ((uint8)length + 3) & 252;
            }
            
            int i;
            for ( i = length; i < padding; ++i ) {
                buffer[i] = (i + (char)(buffer[i - length] ^ 63)) % 26 + 65;
            }
            
            length = i;
            buffer[i] = 0;

        }

        int v16 = CRC(buffer, length);
        int v17 = v16;
        StrUtil::strrev(buffer);

        int v1 = CRC(buffer, length);
        v16 ^= v1;
        v16 ^= v17;

        StrUtil::strrev(buffer);

        for ( int i = 0; i < length; ++i ) {
            v16 ^= buffer[i];
            int v2 = v16;
            buffer[i] ^= v16;
            v16 >>= 8;
            v16 |= v2 << 24;
        }

        for ( int i = 0; i < length; ++i ) {
            buffer[i] |= _addbits[i & 7];
            buffer[i] &= ~_lossbits[i & 7];
        }

        for ( int i = 0; i < length; i += 4 ) {
            sint16 ch1 = buffer[i];
            sint16 ch2 = buffer[i + 1];
            sint16 ch3 = buffer[i + 2];
            sint16 ch4 = buffer[i + 3];
            sint16 tmp1 = ch3 * (ch1 * (ch1 * ch1 ^ (uint16)(2 * ch3)) + (ch4 * ch4 ^ (uint16)(2 * ch2)));
            sint16 tmp2 = tmp1 + ch1 * (ch1 * ch1 ^ (uint16)(2 * ch3));
            buffer[i] = tmp1 ^ ch1 * (uint8)ch1;
            buffer[i + 1] = 2 * ch3 ^ tmp1;
            buffer[i + 2] = 2 * ch2 ^ tmp2;
            buffer[i + 3] = tmp2 ^ ch4 * (uint8)ch4;
        }
    
        return CRC(buffer, length);
        
    }

    return 0;

}

/*******************************************************************************
 * NAME:         Obscure()
 * DESCRIPTION:  <function overview>
 * PARAMETERS:   none
 * RETURN:       none
 * WARNINGS:     none
 *============================================================================*/
sint32 Obscure(char const *string)
{
    //some type of Mersenne Twister??
    //EA::StdC::RandomMersenneTwister::Hash is quite close
    return string[6] + 10 * (string[5] + 10 * (string[4] + 10 * (string[3] + 10 * (string[1] + 10 * string[0])))) - OBSCURE_MAGIC_NUM;
}
