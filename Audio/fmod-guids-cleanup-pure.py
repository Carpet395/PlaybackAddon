# point at a .bank as `python3 fmod-guids-cleanup-pure.py my_audio.bank`
# supports unix wildcards (eg. `python3 fmod-guids-cleanup-pure.py Audio/*.bank`)

import sys
from pathlib import Path
import mmap

def encode_guid(guid):
  guid = guid.replace('-', '')
  b = bytes.fromhex(guid)
  return b[3::-1] + b[5:3:-1] + b[7:5:-1] + b[8:16]

for file in sys.argv[1:]:
  bank_path = Path(file)
  guids_path = Path(file.replace('.bank', '.guids.txt'))

  assert bank_path.is_file()
  assert guids_path.is_file()

  keep_lines = []

  with open(guids_path, 'r', encoding='utf8') as guids_file:
    for line in guids_file:
      guid = line.strip()[1:37]
      encoded_guid = encode_guid(guid)
      with open(bank_path, 'r+b') as bank:
        mm = mmap.mmap(bank.fileno(), 0)
        if mm.find(encoded_guid) != -1:
          keep_lines.append(line)

  print(f'{bank_path.name}: found {len(keep_lines)} GUIDs in use')

  with open(guids_path, 'w', encoding='utf8') as guids_file:
    for line in keep_lines:
      guids_file.write(line)

  print(f'{bank_path.name}: wrote to {guids_path.name}, done')