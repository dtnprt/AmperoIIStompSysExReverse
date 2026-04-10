# Ampero II Stomp SysEx Reverse Engeneering

## Messages Sent By Device

#### Patch Information
```plain

Byte 9 = Package Length? 
    03-05?
        @05 The Name Starts @98 instead of @100
Byte 10-15 = Message Type? 
    00 00 00 00 00 00 = Patch Information (General)
    39 01 xx xx xx xx = Patch Information (Detail)
    72 02 02 03 0E 0C = Patch Information (Even More)

Bytes 32-35 = Bank Index Format: (34 << 12) + (35 << 8) + (32 << 4) + 33
Bytes 100-132 = Patch Name (Nibbels to ASCII)

```

#### Settinges
```plain
    21 25 4D 50 00 00 68 12 10 00 00 00 00 01 00 00 00 02 00 00 00 08 00 00 00 00 00 00 01 05 0F 0F 0F 0F 0F 0F 0F 0F 01 01 00 00 00 00 F7

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

