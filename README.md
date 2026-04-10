# Ampero II Stomp SysEx Reverse Engeneering

# Bank Switch Message Part 1:
```plain
Start: 0
Bytes 10-15 = Message Type? 
    00 00 00 00 00 00 == Patch Information?
    00 00 00 01 00 00 == ??
    00 00 00 03 00 00 == ??
    39 01 xx xx xx xx == ??
    72 02 02 03 0E 0C == ??

Bytes 32-35 = Bank Index Format: (34 << 12) + (35 << 8) + (32 << 4) + 33
Bytes 100-132 = Patch Name (Nibbels to ASCII)
```
