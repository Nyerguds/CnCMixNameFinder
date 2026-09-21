# CnCMixNameFinder - Version history

Note that up until v1.0.5, no version information was actually added into the executable, and the first version was a simple hash calculator for ROL1 called MissionNameChecker. The versions have been retroactively added when constructing this Git repository, so they build with the version numbers included. However, the downloads are the original releases, meaning they do not contain this version information.

## v1.0.0.0

* Original project called MissionNameChecker.
* Simple dialog to calculate name IDs for C&C1/RA1 mix files.

v1.0.1.0

* Project renamed to CnCMixNameFinder.
* Expanded to brute force tool.

## v1.0.2.0

* Added CRC32 (TS), Poor Man's (setup.mix), and dummies for Blade Runner and LOL3.
* Added new program icon.

## v1.0.3.0

* Added back the original hash tester from v1.0.0.0, openable by double-clicking the label in front of the hash method dropdown.

## v1.0.4.0

* Added input field for characters to brute-force.
* Replaced code for CRC32 with Eugene Larchenko's ParallelCRC.
* Disabled unimplemented HashBR and HashLoL3.

## v1.0.5.0

* Implemented LoL3 hasher as HashROR.

## v1.0.6.0

* Refactored ROL algorithm to support both ROL1 (C&C1/RA1) and ROL3 (TS/RA2 setup).

## v1.0.6.1

* Fixed an issue in ROL3, replicating a bug in the algorithm that the actual games have.
* Made the hash test tool more obvious, with a visible box around the label to click, a hand-cursor when hovering over it, and noly requiring a single click.

## v1.0.7.0

* Support for dictionary attacks, and for searching for multiple IDs at once.

## v1.0.8.0

* Added Sole Survivor flavour of the Obfuscate algorithm.
* Cleaned up and refactored a lot of the code.
