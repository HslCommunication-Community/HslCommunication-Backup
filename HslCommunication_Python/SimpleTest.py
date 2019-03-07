
import struct
from HslCommunication import ByteTransform

byteTransform = ByteTransform()
values = [1234, 51235, 2314, 5]
print(byteTransform.UInt16ArrayTransByte(values))

print(struct.pack('<H',values))