arm-eabi-as -mcpu=arm9tdmi -mno-fpu -mthumb -EL -o branch.o branch.s
arm-eabi-ld -Ttext 0x08000000  -EL -e main branch.o 
arm-eabi-objcopy -O binary a.out branch.gba

arm-eabi-as -mcpu=arm7tdmi -mno-fpu -mthumb -EL -o branch2.o branch2.s
arm-eabi-ld -Ttext 0x08000000  -EL -e main branch2.o 
arm-eabi-objcopy -O binary a.out branch2.gba
pause
