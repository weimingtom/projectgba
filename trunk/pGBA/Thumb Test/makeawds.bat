SET OLDPATH=C:\devkitPro\old\devkitarm\bin
%OLDPATH%/arm-elf-as -mcpu=arm9tdmi -mno-fpu -mthumb -EL -o test.o test.s
%OLDPATH%/arm-elf-ld -Ttext 0x08000000  -EL -e main test.o 
%OLDPATH%/arm-elf-objcopy -O binary a.out test.bin
pause