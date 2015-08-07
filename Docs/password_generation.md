# How to create familiar's keys
In the Familiar shelter / gutter there is a menu that **generates a key**. Pressing 'X' when you try to store or retrieve a familiar on the shelter a small dialog appear with some random text. **Nobody knows the meaning**, there is no information on the Internet and probably is a **forgotten feature**. There are some [hidden image](http://gbatemp.net/threads/spanish-v1-0-released-ninokuni-shikkoku-no-madoushi-translation-project.310214/page-34#post-5176232) on the game, probably of the Demo version, with buttons to *Insert a key*, but they do not appear in the final game. As you can read in this document, the key **contains information about the familiar** and some random numbers. In my opinion, it was a final-not-implemented-feature **to share familiar globally**, sharing codes, without Internet connection, similar to save-keys when there wasn't save memories on video games or like *Pokémon Mystery Dungeon* to rescue another user.

1. [Copy stats from familiar](#copy-stats-from-familiar)
2. [Add random number](#add-random-number)
3. [Reverse](#reverse)
4. [Encrypt](#encrypt)
5. [Append CRC](#append-crc)
6. [Encrypt with CRC](#encrypt-with-crc)
7. [Swap](#swap)
8. [Convert to text](#convert-to-text)
9. [Change of alphabet](#change-of-alphabet)

## Copy stats from familiar
Firstly and most important, the game copies familiar's stats. This is the purpose of the password and the last step when doing the reverse operation. The information to copy is the following. Beware it copies bits, not bytes.

| Field          | Bits  | Notes |
| -------------- |:---- :| ----- |
| Name           | 4 * 8 | Copy only the first 4 characters. |
| Level          | 7     | |
| Unknown        | 3     | It's bits 1-3 from 0x27 save familiar structure. |
| Index          | 10    | |
| HP             | 7     | Divided by 8, less precision but higher values. |
| MP             | 7     | Divided by 8. |
| Attack         | 7     | Divided by 8. |
| Deffense       | 7     | Divided by 8. |
| Magic Attack   | 7     | Divided by 8. |
| Magic Deffense | 7     | Divided by 8. |
| Hability       | 7     | Divided by 8. |

## Add random number
Next step is to add some random bits. The game implements a *pseudo-random generator* for 32-bits values. In this case, it updated the *pseudo-random* number and take 11 bits, skipping the first 16 bits, to write in our stream. That is:
```
random_number = update_random_number()
random_number >>= 16
stream.write_bits(random_number, 11)
```

The *pseudo-random number generator* is initialize with a 4-bytes pseudo-random number (TODO: figure out how). Each time it's used, the number is updated as follow. The constants values are set at the initialization (function `0x020D13A4`) and the three values are stored at `0x02143100`.
```
CONSTANT1 = 0x5D588B65
CONSTANT2 = 0x269EC3
new_random_number = random_number * CONSTANT1 + CONSTANT2
```

## Reverse

## Encrypt

## Append CRC

## Encrypt with CRC

## Swap

## Convert to text
The game convert the current 64-bits value to a string with a dictionary / alphabet. First it does the modulo operation to get the index of the char and then divide the value until it become 0. The *alphabet1* is used with length `0x3A` (58 chars) and 8-bits per char.
```python
ALPHABET1 = "0123456789abcdefghijkmnpqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ"
```

The process in *Python* is the following (*Programs/script/Password.py*):
```python
text = ''
while data != 0:
    index = data % 0x3A
    text += ALPHABET1[index]
    data /= 0x3A
```

Once the password is in text format, the game checks if it can get back the value.
```
data_i = data_i-1 + (char_i_index * alphabet1_len^i)
```

In python (*Programs/scripts/Password.py*) it would be:
```python
ITERATION = 0xB
PASSWORD = "" # 11 password-len here

data = 0
for i in range(ITERATION):
    idx = ALPHABET1.find(PASSWORD[i])
    data = data + (idx * (len(ALPHABET1) ** i))
```

## Change of alphabet
The last operation is to mess the current password replacing each char with another. To do so, it's used the function `0x020C8BD0` with the following steps.

1. Check input is not null.
2. For each char:
    1. Get the index of char in the **alphabet 1**. In this alphabet     each char is one-byte. It's at `0x020CC8E8`. The size is fixed: 56 chars.
    2. Get the char in that index from the **alphabet 2**. In this case, there are two bytes per char. It's at `0x020CC924`. The size is variable, until found a null char.
    3. Write the output char.
3. Write a null char at the end.

```python
ALPHABET2 = ""
```

For the Spanish translation with the hack *Assembly/password/alphabet.asm* we replace the *alphabet 2* Japanese kanjis with ASCII chars and some UNICODE symbols like arrows and circles. The game continue to read two bytes to get a char, so if it's the index is in the ASCII part it replace one with two char.
A UNICODE char must start in an even position.