# CnCMixNameFinder - Version history

Note that up until v1.0.5, no version information was actually added into the executable, and the first version was a simple hash calculator for ROL1 called MissionNameChecker. The versions have been retroactively added when constructing this Git repository, so they build with the version numbers included. However, the downloads are the original releases, meaning they do not contain this version information.

## v1.0.0.0

Release date: 2011-08-19 19:23

* Original project called MissionNameChecker.
* Simple dialog to calculate name IDs for C&C1/RA1 mix files.

## v1.0.1.0

Release date: 2014-01-27 18:24

* Project renamed to CnCMixNameFinder.
* Expanded to brute force tool.

## v1.0.2.0

Release date: 2016-04-20 15:03

* Added CRC32 (TS), Poor Man's (setup.mix), and dummies for Blade Runner and Lands of Lore 3.
* Added new program icon.

## v1.0.3.0

Release date: 2016-04-20 17:25

* Added back the original hash tester from v1.0.0.0, openable by double-clicking the label in front of the hash method dropdown.

## v1.0.4.0

Release date: 2016-11-16 09:10

* Added input field for characters to brute-force.
* Replaced code for CRC32 with Eugene Larchenko's ParallelCRC.
* Disabled unimplemented Blade Runner and Lands of Lore 3 hashers.

## v1.0.5.0

Release date: 2016-11-21 10:13

* Implemented Lands of Lore 3 hasher as "ROR".
* Added a frame on the hash method label, to indicate it can be double-clicked to open the hash test tool.

## v1.0.6.0

Release date: 2022-02-26 18:27

* Refactored ROL algorithm to support both ROL1 (C&C1/RA1) and ROL3 (TS/RA2 setup).

## v1.0.6.1

Release date: 2022-02-26 18:40

* Fixed an issue in ROL3, replicating a bug in the algorithm that the actual games have.
* Made the hash test tool more obvious by showing a hand-cursor when hovering over it, and only requiring a single click to open.

## v1.0.7.0

Release date: 2023-11-21 19:49

* Support for dictionary attacks, and for searching for multiple IDs at once.

## v1.0.8.0

Release date: 2026-03-25 12:42

* Added Sole Survivor flavour of the Obfuscate algorithm.
* Cleaned up and refactored a lot of the code.

## v1.0.8.1

Release date: 2026-09-21 14:11

* Updated to .Net Framework 4.8.1.
* Fixed typo in the names of the "Obfuscate" methods.
* Added line at the start of the log to give info about what hashing type is being performed.
* Switched to exclusively building "any CPU" builds.
