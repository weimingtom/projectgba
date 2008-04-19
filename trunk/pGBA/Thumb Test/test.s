
.text
.thumb

@Opcode Test Rom

@Section 1
lsl	r0,r1,#2
lsr r0,r2,#1
asr r2,r1,#5

@Section 2
add r0,r0,r1
sub r2,r0,r1
add r1,r0,#6
sub r3,r2,#2

@Section 3
mov r0, #3
cmp r0, #2
add r1, #1
sub r0, #1

@Section 4
and r0,r1
eor r0,r1
lsl r0,r1
lsr r0,r1
asr r0,r1
adc r0,r1
sbc r0,r1
ror r0,r1
tst r0,r1
neg r0,r1
cmp r0,r1
cmn r0,r1
orr r0,r1
mul r0,r1
bic r0,r1
mvn r0,r1


.end
