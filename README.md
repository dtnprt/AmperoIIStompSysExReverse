# Ampero II Stomp SysEx Reverse Engeneering

## Format
* 1 byte, `F0`: SysEx start
* 6 bytes, `21 25 4D 50 00 00`: Start of every packet
* 1 byte,  `xx`:  unknown (CheckSum?)
* 1 byte:  `11` or `12` for Send/Return SysEx messages
* 2 bytes: payload length of the converted payload in little endian
* 2 bytes: payload offset of the converted payload in little endian?
* ... : Payload in nibbles (0x, 0y > xy)
* 1 byte, `F7`: SysEx end


## SysEx Commands

#### Get Firmware Version
```plain
F0 21 25 4D 50 00 00 08 11 08 00 00 00 00 05 00 00 00 00 00 03 00 00 00 00 00 00 00 00 F7
```
#### Get Current Patch and Scene Information
```plain
F0 21 25 4D 50 00 00 09 11 08 00 00 00 00 00 00 00 00 09 00 00 00 00 00 00 00 00 00 00 F7
```
#### Set a patch and get information???
```plain
F0 21 25 4D 50 00 00 13 11 10 00 00 00 00 00 00 00 00 00 00 01 00 08 00 00 00 00 00 00 01 05 00 02 00 00 00 00 00 00 01 01 00 00 00 00 F7
``` 
#### Get Current Scene
```plain
F0 21 25 4D 50 00 00 04 11 08 00 00 00 00 01 00 00 00 00 00 03 00 00 00 00 00 00 00 00 F7
```
#### Get Patch Overview
```plain
F0 21 25 4D 50 00 00 09 11 08 00 00 00 00 00 00 00 00 08 00 01 00 00 00 00 00 00 00 00 F7
```
#### Get User IRs
```plain
F0 21 25 4D 50 00 00 07 11 08 00 00 00 00 03 00 00 00 00 00 04 00 00 00 00 00 00 00 00 F7
```

#### Get Clones
```plain
F0 21 25 4D 50 00 00 0A 11 08 00 00 00 00 03 00 00 00 00 00 07 00 00 00 00 00 00 00 00 F7
```

#### ???
```plain
F0 21 25 4D 50 00 00 0A 11 08 00 00 00 00 00 00 00 00 0A 00 00 00 00 00 00 00 00 00 00 F7

F0 21 25 4D 50 00 00 06 11 08 00 00 00 00 03 00 00 00 00 00 03 00 00 00 00 00 00 00 00 F7

F0 21 25 4D 50 00 00 0D 11 08 00 00 00 00 0A 00 00 00 00 00 03 00 00 00 00 00 00 00 00 F7

F0 21 25 4D 50 00 00 09 11 08 00 00 00 01 03 00 00 00 00 00 05 00 00 00 00 00 00 00 00 F7

F0 21 25 4D 50 00 00 07 11 08 00 00 00 00 00 00 00 00 07 00 00 00 00 00 00 00 00 00 00 F7

F0 21 25 4D 50 00 00 1E 11 10 00 00 00 00 03 00 00 00 0A 00 01 00 08 00 00 00 00 00 00 01 05 00 00 00 00 00 00 00 00 01 01 00 00 00 00 F7
```


## Messages Sent By Device

#### Patch Information
```plain
Payload Starts on 0x0d on every packet



```

#### Settinges
```plain
    F0 21 25 4D 50 00 00 68 12 10 00 00 00 00 01 00 00 00 02 00 00 00 08 00 00 00 00 00 00 01 05 0F 0F 0F 0F 0F 0F 0F 0F 01 01 00 00 00 00 F7

    Byte 17:    Category
    Byte 10-15: Setting
    Byte 30-37: Value

    Categories:
        01 = I/O
        02 = USB Audio
        06 = Expression (Value @ 34)
        07 = Controls
        08 = Display
        09 = Volume
        ...

    00 00 00 01 00 00 00 01 = Input Model(L) (0x00 = E.GT, 0x01 = A.GT, 0x02 = Line)
    00 00 00 02 00 00 00 01 = Input Model(R) (0x00 = E.GT, 0x01 = A.GT, 0x02 = Line)
    00 00 00 02 00 00 00 08 = Patch Mode 1/2
    00 00 00 03 00 00 00 01 = Unit Mode Stomp/Patch
    00 00 00 0D 00 00 00 08 = Stomp Mode 1/2
    00 00 00 05 00 00 00 08 = Language
    00 00 00 0E 00 00 00 01 = Output Level (Inst/Line)




### Other

#### On block change:
* `0x07` (converted): Slot Number

```plain
(i) received unknown block:
       00 01 02 03 04 05 06 07  08 09 0a 0b 0c 0d 0e 0f    0123456789abcdef
0x00   21 25 4d 50 00 00 3f 12  14 00 00 00 00 03 00 00    !%MP··?·········
0x10   00 0a 00 01 00 0c 00 00  00 00 00 00 01 09 00 00    ················
0x20   00 00 00 00 00 00 00 00  08 00 01 03 04 04 01 01    ················
0x30   00 00 00 00 f7                                      ····÷

(i) converted payload:
       00 01 02 03 04 05 06 07  08 09 0a 0b 0c 0d 0e 0f    0123456789abcdef
0x00   0a 01 0c 00 00 00 19 xx  00 00 00 00 80 13 44 11    ············?·D·
0x10   00 00                                               ··
```

#### Block switch on/off

*    `0x07` (converted): Block ID
*    `0x0d` (converted): State

```plain
(i) received unknown block:
       00 01 02 03 04 05 06 07  08 09 0a 0b 0c 0d 0e 0f    0123456789abcdef
0x00   21 25 4d 50 00 00 3b 12  13 00 00 00 00 02 00 00    !%MP··;·········
0x10   00 04 00 01 00 0b 00 00  00 00 00 00 01 08 00 00    ················
0x20   00 01 00 00 00 00 00 00  00 01 00 00 01 01 00 00    ················
0x30   00 00 f7                                            ··÷
(i) converted payload:
       00 01 02 03 04 05 06 07  08 09 0a 0b 0c 0d 0e 0f    0123456789abcdef
0x00   04 01 0b 00 00 00 18 00  01 00 00 00 01 00 11 00    ················
0x10   00                                                  ·

(i) received unknown block:
       00 01 02 03 04 05 06 07  08 09 0a 0b 0c 0d 0e 0f    0123456789abcdef
0x00   21 25 4d 50 00 00 3b 12  13 00 00 00 00 02 00 00    !%MP··;·········
0x10   00 04 00 01 00 0b 00 00  00 00 00 00 01 08 00 00    ················
0x20   00 01 00 00 00 00 00 00  00 01 00 01 01 01 00 00    ················
0x30   00 00 f7                                            ··÷
(i) converted payload:
       00 01 02 03 04 05 06 07  08 09 0a 0b 0c 0d 0e 0f    0123456789abcdef
0x00   04 01 0b 00 00 00 18 00  01 00 00 00 01 01 11 00    ················
0x10   00    
```