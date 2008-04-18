
.text
.thumb

@Opcode Test Rom

@Section 1
lsl	r0,r1,#2
lsr r0,r2,#1
asr r2,r1,#5

@Section 2
add r0,r0,r1
add r1,r0,#6
sub r3,r2,r1
sub r3,r2,#2

@Section 3
mov r0, #3
cmp r0, #2
add r1, #1
sub r0, #1


.end
