using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crunchy
{
    public class Chip
    {
        /// <summary>
        /// The chips main memory 4KB total.
        /// </summary>
        private byte[] ram = Array.Empty<byte>();

        /// <summary>
        /// Display buffer for drawing graphics.
        /// </summary>
        public byte[] displayBuffer = Array.Empty<byte>();

        /// <summary>
        /// The program counter for pointing at the current instruction.
        /// </summary>
        private ushort programCounter = 0x200;

        /// <summary>
        /// Special index register used to point at locations in RAM.
        /// </summary>
        private ushort indexRegister = 0x200;

        /// <summary>
        /// The stack.
        /// </summary>
        private Stack<ushort> stack = new();

        /// <summary>
        /// 8 bit delay timer that is decremented 60 times a second to 0.
        /// </summary>
        private byte delayTimer = 0;

        /// <summary>
        /// Same as the delay timer but used for sound.
        /// </summary>
        private byte soundTimer = 0;

        /// <summary>
        /// The general purpose registers.
        /// </summary>
        private byte[] registers = Array.Empty<byte>();

        /// <summary>
        /// Default constructor.
        /// </summary>
        public Chip()
        {
            ram = new byte[4096];
            displayBuffer = new byte[4096];
            registers = new byte[16];
        }

        /// <summary>
        /// Loads the rom into ram.
        /// </summary>
        /// <param name="rom">Byte array of the program rom.</param>
        /// <returns>Returns a boolean indicating success.</returns>
        public bool LoadRom(byte[] rom)
        {
            var romSize = rom.Length;

            // Make sure the rom can fit in the 4096 bytes of ram minus
            // the reserved 512 bytes used for fonts.
            if (romSize > (4096 - 512))
            {
                return false;
            }

            for (int i = 512; i < 512 + romSize; i++)
            {
                ram[i] = rom[i - 512];
            }

            return true;
        }

        /// <summary>
        /// Step the chip forward once.
        /// </summary>
        public void Step()
        {
            var currentInstruction = GetInstruction();

            byte firstNibble = (byte) ((currentInstruction & 0xF000) >> 8);

            switch (firstNibble)
            {
                case 0x00:
                    Array.Clear(displayBuffer);
                    break;

                case 0x10:
                    programCounter = (ushort)(currentInstruction & 0x0FFF);
                    break;

                case 0x60:                    
                    registers[GetRegister(currentInstruction)] = (byte)(currentInstruction & 0x00FF);
                    break;

                case 0x70:
                    registers[GetRegister(currentInstruction)] += (byte)(currentInstruction & 0x00FF);
                    break;

                case 0xA0:
                    indexRegister = (ushort)(currentInstruction & 0x0FFF);
                    break;
                
                // Sprites are always 8 pixels wide, and 1 to 15 pixels tall.
                case 0xD0:
                    byte xLocation = registers[GetXDrawingRegister(currentInstruction)];
                    byte yLocation = registers[GetYDrawingRegister(currentInstruction)];
                    byte byteCount = GetDrawingByteCount(currentInstruction);

                    for (int i = 0; i < byteCount; i++)
                    {
                        byte spriteByte = ram[indexRegister + i];

                        for (int j = 0; j < 8; j++)
                        {
                            byte spriteBit = (byte) (spriteByte & (0x80 >> j));
                            ushort destinationAddress = (ushort) ((yLocation + i) * 0x40 + (xLocation + j));

                            if (spriteBit != 0)
                            {
                                if (displayBuffer[destinationAddress] == 0xFF)
                                {
                                    registers[0x0F] = 1;
                                }

                                displayBuffer[destinationAddress] ^= 0xFF;
                            }                            
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// 0x00E0 - Clear Screen
        /// </summary>
        /// <returns></returns>
        private ushort GetInstruction()
        {
            var instruction = (ushort) ((ram[programCounter] << 8) + ram[programCounter + 1]);
            programCounter+=2;

            return instruction;
        }

        /// <summary>
        /// Returns the register referenced in the current instruction.
        /// </summary>
        /// <param name="currentInstruction">The current instruction.</param>
        /// <returns>Returns the current register.</returns>
        private byte GetRegister(ushort currentInstruction)
        {
            return (byte) ((currentInstruction & 0x0F00) >> 8);
        }

        private byte GetXDrawingRegister(ushort currentInstruction)
        {
            return (byte) ((currentInstruction & 0x0F00) >> 8);
        }

        private byte GetYDrawingRegister(ushort currentInstruction)
        {
            return (byte) ((currentInstruction & 0x00F0) >> 4);
        }
        private byte GetDrawingByteCount(ushort currentInstruction)
        {
            return (byte)(currentInstruction & 0x000F);
        }
    }
}
