#include "string"
#include "file.h"

typedef unsigned int uint32;

unsigned int __declspec(naked) __cdecl Calculate_CRC(char *str, unsigned int length)
{
    _asm {
                enter   4, 0
                push    esi
                mov     dword ptr [ebp-4], 0
                pusha
                mov     esi, [ebp+8]
                cld
                xor     ebx, ebx
                mov     ecx, [ebp+0Ch]
                jecxz   short loc_9FB4D

                mov     edx, ecx
                and     dl, 3
                shr     ecx, 2
                jecxz   short loc_9FB29

    loc_9FB22:
                lodsd
                rol     ebx, 1
                add     ebx, eax
                loop    loc_9FB22

    loc_9FB29:
                or      dl, dl
                jz      short loc_9FB4D

                mov     ecx, edx
                xor     eax, eax
                and     ecx, 0FFFFh
                push    ecx

    loc_9FB38:
                lodsb
                ror     eax, 8
                loop    loc_9FB38

                pop     ecx
                neg     ecx
                add     ecx, 4
                shl     ecx, 3
                ror     eax, cl
                rol     ebx, 1
                add     ebx, eax

    loc_9FB4D:
                mov     [ebp-4], ebx
                popa
                mov     eax, [ebp-4]
                pop     esi
                leave
                retn
    }
}

void Add_CRC(unsigned int *crc, unsigned int hash)
{
  bool v3 = (*((unsigned char *)crc + 3) & 128) != 0;
  *crc *= 2;
  *crc |= v3;
  *crc += hash;
}

int Calculate_String_CRC(char *source)
{
    unsigned int tmp = 0;
    
    unsigned int crc = 0;
    size_t len = strlen(source);
    char *src = source;
    
    while (4 < len) {
        tmp = *(unsigned int *)src;
        Add_CRC(&crc, tmp);
        src += 4;
        len -= 4;
    }
    
    if (0 < (signed int)len) {
        tmp = 0;
        memcpy(&tmp, src, len);
        Add_CRC(&crc, tmp);
    }

    return crc;
}

int Obfuscate(char *string)
{
    int i;
    
    if (string == nullptr) {
        return 0;
    }

    char buffer[128];

    //memset(buffer, 0xA5, sizeof(buffer)); // useless?......

    strncpy(buffer, string, sizeof(buffer));
    buffer[sizeof(buffer) - 1] = 0;

    int length = strlen(buffer);
    //strupr(buffer);

    for ( i = 0; i < length; ++i ) {
        if (!isgraph(buffer[i])) {
            buffer[i] = i % 26 + 'A';
        }
    }
    
    
    if ( length < 16 || length & 3 ) {
        int padding = 16;

        if ( (((unsigned char)length + 3) & 252) > 16 ) {
            padding = ((unsigned char)length + 3) & 252;
        }

        for ( i = length; i < padding; ++i ) {
            buffer[i] = (i + (char)(buffer[i - length] ^ 63)) % 26 + 'A';
        }

        length = i;
        buffer[i] = 0;
    }

    // useless......
    //int hash = Calculate_String_CRC(buffer);
    //int v16 = hash;
    
    strrev(buffer);
    int rev_hash = Calculate_CRC(buffer, length);

    //hash ^= rev_hash;
    //hash ^= v16;
    
    
    //ghidra shows above as this, xor hash1 twice is useless..........
    //hash ^= hash ^ rev_hash;

    //exact same result......
    int hash = rev_hash;
 
    strrev(buffer);

    for ( i = 0; i < length; ++i ) {
        hash ^= (unsigned char)buffer[i];
        buffer[i] ^= hash;
        //unsigned char v2 = hash;
        //hash >>= 8;
        //hash |= v2 << 24;
        
        //ghidra
        hash = hash >> 8 | hash << 24;
    }
    
    static unsigned char _lossbits[8] = {
        0, 8, 0, 32, 0, 4, 16, 0
    };
    
    static unsigned char _addbits[8] = {
        16, 0, 0, 128, 64, 0, 0, 4
    };
    
    for ( i = 0; i < length; ++i ) {
        buffer[i] |= _addbits[i % sizeof(_addbits)];
        buffer[i] &= ~_lossbits[i % sizeof(_lossbits)];
    }
    
    /*
    //original code
    for ( i = 0; i < length; i += 4 ) {
 
        unsigned short ch1 = buffer[i];
        unsigned short ch2 = buffer[i + 1];
        unsigned short ch3 = buffer[i + 2];
        unsigned short ch4 = buffer[i + 3];
        
        unsigned short tmp1 = ch3 * (ch1 * (ch1 * ch1 ^ 2 * ch3) + (ch4 * ch4 ^ 2 * ch2));
        unsigned short tmp2 = tmp1 + ch1 * (ch1 * ch1 ^ 2 * ch3);
        
        buffer[i + 0] = tmp1 ^ ch1 * ch1;
        
        buffer[i + 1] = 2 * ch3 ^ tmp1;
        buffer[i + 2] = 2 * ch2 ^ tmp2;
        
        buffer[i + 3] = tmp2 ^ ch4 * ch4;
    }
    */
    
    ///*
    //rewrite to reduce repeats
    for ( i = 0; i < length; i += 4 ) {
        
        unsigned short ch1 = buffer[i + 0];
        unsigned short ch2 = buffer[i + 1];
        unsigned short ch3 = buffer[i + 2];
        unsigned short ch4 = buffer[i + 3];
        
        //printf("    %08X\n", *(unsigned int *)&buffer[i + 0]);
        
        unsigned short tmp5 = 2 * ch3;
        unsigned short tmp4 = 2 * ch2;
        
        unsigned short tmp6 = ch1 * ch1;
        unsigned short tmp3 = ch4 * ch4;
        
        unsigned short tmp0 = ch1 * (tmp6 ^ tmp5);
        unsigned short tmp1 = ch3 * (tmp0 + (tmp3 ^ tmp4));
       
        unsigned short tmp2 = tmp1 + tmp0;
        
        buffer[i + 0] = tmp1 ^ tmp6;
        buffer[i + 1] = tmp5 ^ tmp1;
        buffer[i + 2] = tmp4 ^ tmp2;
        buffer[i + 3] = tmp2 ^ tmp3;
        
        //printf("    %08X\n\n", *(unsigned int *)&buffer[i + 0]);
    }
    //*/

    //ghidra
    /*
    for ( i = 0; i < length; i += 4 ) {
 
      short bVar0 = buffer[i + 0];
      short bVar1 = buffer[i + 2] * 2;
      short bVar2 = buffer[i + 1] * 2;
      short res3 = buffer[i + 3] * buffer[i + 3];
      
      short cVar2 = (bVar1 ^ bVar0 * bVar0) * bVar0;
      
      short res1 = ((bVar2 ^ res3) + cVar2) * buffer[i + 2];
      short res2 = cVar2 + res1;
      short res0 = bVar0 * bVar0 ^ res1;
      
      res3 ^= res2;
      res1 ^= bVar1;
      res2 ^= bVar2;
      
      buffer[i + 0] = res0;
      buffer[i + 1] = res1;
      buffer[i + 2] = res2;
      buffer[i + 3] = res3;
    }
    */

    return Calculate_CRC(buffer, length);
}
//order of freqency
//s a c m p r t b f g d h i n e l o w u v j k q y z x
static const char alphabet[] = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
//static const char alphabet[] = "SACMPRTBFGDHINELOWUVJKQYZX ";
static const int alphabet_size = sizeof(alphabet) - 1;

//unknown RA hashes
uint32 const UnkCodesRA[] = { 0x72A47EF6, 0xD95C68A2 };

uint32 const PlayCodes[] = { 0x90046ECF };
uint32 const CheatCodes[] = { 0xDFABC23A, 0xBE79088C };

//unknown SS hashes
uint32 const UnkCodesSS[] = { 0x51842BF3, 0xC87AD5A4, 0x7E6AB9E8 };

//unknown C&C, the inikey on the right that loads a string and checks the string agaisnt the hash on the left
uint32 const OptionsCC[] = {
    0xB1A34435,  //TrueNames
    0xF7867BF0,  //Bibs
    0xDC57C4B2,  //Combat
    0xACB58F61,  //Sounds

};uint32 const OptionsR[] = {
    0x39D01821,
    0x2E7FE493,
    0x7E7C4CCA,
};

/*
    //PlayCodes
    0xE0792D6D = SONY
    0xC3EE9A26 = FUNK
    0xED382178 = SLICK
    
    //CheatCodes
    0xA0E2AB53 = JUPITER
    0x00532693 = NADE
                 NATE
    0x7DDFF824 = PASSWORD
    0x2CB5CF01 = CHEATER
    0xB5B63531 = BLUB
    0x52B19A22 = NUKE
    0xB216AE7E = SPOON
    0x0E07B213 = CARPET

    //EditorCodes
    0xA2C09326 = BUILD
    0x1F944BB3 = MOBIUS
    0xDE07154D = CYCLONE
    0x0E07B213 = CARPET
    0x16B170B1 = EDITOR

    //CC95 option hashes
    0x03552894 = DANCING  		//Rotation		- Use three point turn logic.
    0x53EBECBC = A LA CARTE		//Helipad		- Allow separate helipad purchase
    0x104DF10F = TRANSFORMER	//MCV			- Allow undeploy of construction yard.
    0x00AB6BEF = LIGHTNING		//TreeTarget	- Allow targeting of trees.
    0x7FDE2C33 = REMIX			//Scores
    0x9E3881B8 = SERGENT		//CombatIQ
    0x4EA2FBDF = PANCAKE		//Overrun
    0xC084AE82 = RESTRICTED		//Scrolling
    0x5D9F6F24 = 6				//Players
    
    //CC95 cmd parse hashes
    0x59E975CE = EASY
    0xACFE9D13 = HARD
    0x11CA05BB = FUNPARK
*/

void check(char *string)
{
    bool found = false;

    
    //string[strlen(string) - 1] = 0x20;
    
    uint32 hash = Obfuscate(string);
    printf("                                                           \r", string);
    printf("trying \"%s\"\r", string);
    
    for ( int i = 0; i < 2; ++i) {
        if (UnkCodesRA[i] == hash) {
            printf("String %s matches UnkCodesRA Hash %08x\n", string, hash);
            found = true;
            break;
        }
    }
    
    if (found) return;
    
    for ( int i = 0; i < 1; ++i) {
        if (PlayCodes[i] == hash) {
            printf("String %s matches Play Hash %08x\n", string, hash);
            found = true;
            break;
        }
    }
    
    if (found) return;
    
    for ( int i = 0; i < 2; ++i) {
        if (CheatCodes[i] == hash) {
            printf("String %s matches Cheat Hash %08x\n", string, hash);
            found = true;
            break;
        }
    }
    
    if (found) return;
    
    /*for ( int i = 0; i < 1; ++i) {
        if (EditorCodes[i] == hash) {
            printf("String %s matches Editor Hash %08x\n", string, hash);
            found = true;
            break;
        }
    }
    
    if (found) return;*/
    
    for ( int i = 0; i < 3; ++i) {
        if (UnkCodesSS[i] == hash) {
            printf("String %s matches UnkCodesSS %08x\n", string, hash);
            found = true;
            break;
        }
    }
    
    if (found) return;
    
    for ( int i = 0; i < 5; ++i) {
        if (OptionsCC[i] == hash) {
            printf("String %s matches OptionsCC %08x\n", string, hash);
            found = true;
            break;
        }
    }
    
    for ( int i = 0; i < 5; ++i) {
        if (OptionsR[i] == hash) {
            printf("String %s matches TEST %08x\n", string, hash);
            found = true;
            break;
        }
    }
    
    if (found) return;
    
    //printf("NO MATCH String %s Hash %08x\n", string, hash);
    
    return;
}

void brute_impl(char * str, int index, int max_depth)
{
    int i;
    for (i = 0; i < alphabet_size; ++i)
    {
        //str[0] = 'N';
        //str[1] = 'O';
        //str[2] = 'T';
        //str[3] = ' ';
        //str[4] = 'T';
        str[index] = alphabet[i];
        //str[index + 1] = ' ';
        //str[index + 2] = 'I';
        //str[index + 3] = 'A';
        //str[index + 4] = 'L';
        
        if (index == max_depth - 1)
        {
            check(str); // put check() here instead
        }
        else
        {
            brute_impl(str, index + 1, max_depth);
        }
    }
}

void brute_sequential(int max_len)
{
    char * buf = new char(max_len + 8);
    int i;

    for (i = 1; i <= max_len; ++i)
    {
        memset(buf, 0, max_len + 6);
        brute_impl(buf, 0, i);
    }

    delete[] buf;
}

#define DICT_SIZE 869228

int size = 0;
static char dict[DICT_SIZE][256];

void dict_brute1()
{
    char *buf = new char[512];
    
    for (int i = 0; i < size; ++i) {
        snprintf(buf, 512, "%s", dict[i]);
        //printf("%s, gives %08X\n", buf, Obfuscate(buf));
        check(buf);
    }
    delete[] buf;
}

void dict_brute2()
{
    char *buf = new char[512];
    
    for (int i = 0; i < size; ++i) {
        for (int j = 0; j < size; ++j) {
            snprintf(buf, 512, "%s %s", dict[i], dict[j]);
            //printf("%s, gives %08X\n", buf, Obfuscate(buf));
            check(buf);
        }
    }
    delete[] buf;
}




void process_dict()
{;
    char buffer[256];
    FileClass txt("dic-0294.txt");
    txt.Open();
    BOOL end_of_file = false;
    
    unsigned int i = 0;
    for (; i < DICT_SIZE; ++i ) {
        Read_Line(txt, dict[i], 512, end_of_file);    
        if (end_of_file) break;
    }
    size = i;
    dict_brute1();
    
    delete [] dict;
}

int main(int argc, char* argv[])
{
    if ( argc > 1 ) {
        printf("%s is %08X\n", argv[1], Obfuscate(argv[1]));
        check(argv[1]);
    } else {
        //process_dict();
        brute_sequential(8);
        
        char buffer[] = "ABCDEFGHIJKLMNOPQRSTUVWXYZ ";
        
        
        //static unsigned char _lossbits[8] = {
        //    0, 8, 0, 32, 0, 4, 16, 0
        //};
        //
        //static unsigned char _addbits[8] = {
        //    16, 0, 0, 128, 64, 0, 0, 4
        //};
        
        
        
        ////printf("Loss vals are\n");
        ////for ( int i = 0; i < sizeof(buffer); ++i ) {
        ////    printf("%x - ", (unsigned char)buffer[i]);
        ////    buffer[i] |= _addbits[i % sizeof(_lossbits)];
        ////    buffer[i] &= ~_lossbits[i % sizeof(_addbits)];
        ////    printf("%x\n", (unsigned char)buffer[i]);
        ////}
        //
        //
        //int hash = Calculate_String_CRC(buffer);
        //
        //for ( int i = 0; i < sizeof(buffer); ++i ) {
        //    printf("%x\n", hash);
        //    hash ^= (unsigned char)buffer[i];
        //    buffer[i] ^= hash;
        //    //unsigned char v2 = hash;
        //    //hash >>= 8;
        //    //hash |= v2 << 24;
        //    
        //    //ghidra
        //    hash = hash >> 8 | hash << 24;
        //    printf("%x\n", hash);
        //}
       
    }

    return 0;
}
