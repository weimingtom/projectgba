arm-eabi-as -mcpu=arm9tdmi -mno-fpu -mthumb -EL -o branch.o branch.s
arm-eabi-ld -Ttext 0x08000000  -EL -e main branch.o 
arm-eabi-objcopy -O binary a.out branch.gba
pause