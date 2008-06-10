
.text
.thumb

@Opcode Test Rom

@test logging
swi 0xFF

mov r0, #6
mov r1, #24
mov r2, #0
mov r3, #8
lsl r0, r0, r1

loop:
strb r1, [r0]
add	r2, #1
cmp r2, r3
bne loop

forever:
b forever

push {r0-r4}
pop {pc}

STMIA R7!,{R0-R2}
LDMIA R0!,{R1,R5}

.end
