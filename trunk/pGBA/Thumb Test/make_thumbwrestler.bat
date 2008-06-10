arm-eabi-as -mcpu=arm9tdmi -mno-fpu -mthumb -EL -o thumbwrestler.o thumbwrestler.asm
arm-eabi-ld -Ttext 0x08000000  -EL -e main thumbwrestler.o 
arm-eabi-objcopy -O binary a.out thumbwrestler.gba
pause