using MiscUtil.Conversion;
using MiscUtil.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace LibGxFormat.Lz
{
    static class LzssParameters
    {
        /// <summary>Size of the ring buffer.</summary>
        public const int N = 4096;
        /// <summary>Maximum match length for position coding. (0x0F + THRESHOLD).</summary>
        public const int F = 18;
        /// <summary>Minimum match length for position coding.</summary>
        public const int THRESHOLD = 3;
        /// <summary>Index for root of binary search trees.</summary>
        public const int NIL = N;
        /// <summary>Character used to fill the ring buffer initially.</summary>
        //private const ubyte BUFF_INIT = ' ';
        public const byte BUFF_INIT = 0; // Changed for F-Zero GX
    }

    class LzssEncoder
    {
	    // Ring buffer of size N, with extra F-1 bytes to facilitate comparison
	    byte[] ringBuf = new byte[LzssParameters.N + LzssParameters.F - 1];

	    // Match position and length of the longest match. Set by InsertNode().
	    int matchPosition, matchLength;

	    // Binary search trees.
	    int[] left = new int[LzssParameters.N + 1];
	    int[] right = new int[LzssParameters.N + 257];
	    int[] parent = new int[LzssParameters.N + 1];

	    /// Initialize binary trees.
	    void InitTree()
	    {
		    /* For i = 0 to N - 1, right[i] and left[i] will be the right and
		     * left children of node i. These nodes need not be initialized.
		     *
		     * Also, parent[i] is the parent of node i.
		     * These are initialized to NIL (= N), which stands for 'not used'.
		     *
		     * For i = 0 to 255, right[N + i + 1] is the root of the tree
		     * for strings that begin with character i. These are initialized
		     * to NIL. Note there are 256 trees.
		     */

		    for (int i = LzssParameters.N + 1; i <= LzssParameters.N + 256; i++)
			    right[i] = LzssParameters.NIL;

		    for (int i = 0; i < LzssParameters.N; i++)
			    parent[i] = LzssParameters.NIL;
	    }

	    /**
	     * Inserts string of length F, ringBuf[r..r+F-1], into one of the
	     * trees (ringBuf[r]'th tree) and returns the longest-match position
	     * and length via the global variables matchPosition and matchLength.
	     * If matchLength >= F, then removes the old node in favor of the new
	     * one, because the old one will be deleted sooner.
	     * Note r plays double role, as tree node and position in buffer.
	     */
	    void InsertNode(int r)
	    {
		    int  i, p, cmp;
		    int keyIdx;

		    cmp = 1;  keyIdx = r;  p = LzssParameters.N + 1 + ringBuf[keyIdx+0];
		    right[r] = left[r] = LzssParameters.NIL;  matchLength = 0;
		    for ( ; ; ) {
			    if (cmp >= 0) {
				    if (right[p] != LzssParameters.NIL) p = right[p];
				    else {  right[p] = r;  parent[r] = p;  return;  }
			    } else {
				    if (left[p] != LzssParameters.NIL) p = left[p];
				    else {  left[p] = r;  parent[r] = p;  return;  }
			    }
			    for (i = 1; i < LzssParameters.F; i++)
				    if ((cmp = ringBuf[keyIdx+i] - ringBuf[p + i]) != 0)  break;
			    if (i > matchLength) {
				    matchPosition = p;
				    if ((matchLength = i) >= LzssParameters.F)  break;
			    }
		    }
		    parent[r] = parent[p];  left[r] = left[p];  right[r] = right[p];
		    parent[left[p]] = r;  parent[right[p]] = r;
		    if (right[parent[p]] == p) right[parent[p]] = r;
		    else                   left[parent[p]] = r;
		    parent[p] = LzssParameters.NIL;  /* remove p */
	    }

	    /**
	     * Deletes node p from tree.
	     */
	    void DeleteNode(int p) 
	    {
		    int  q;
	
		    if (parent[p] == LzssParameters.NIL) return;  /* not in tree */
		    if (right[p] == LzssParameters.NIL) q = left[p];
		    else if (left[p] == LzssParameters.NIL) q = right[p];
		    else {
			    q = left[p];
			    if (right[q] != LzssParameters.NIL) {
				    do {  q = right[q];  } while (right[q] != LzssParameters.NIL);
				    right[parent[q]] = left[q];  parent[left[q]] = parent[q];
				    left[q] = left[p];  parent[left[p]] = q;
			    }
			    right[q] = right[p];  parent[right[p]] = q;
		    }
		    parent[q] = parent[p];
		    if (right[parent[p]] == p) right[parent[p]] = q;  else left[parent[p]] = q;
		    parent[p] = LzssParameters.NIL;
	    }

        public byte[] Encode(byte[] input)
        {
	        List<byte> app = new List<byte>();
	        int inputPos = 0;

	        int len, r, s, last_matchLength, i;

	        byte[] code_buf = new byte[17];
	        int code_buf_ptr;
	        byte mask;

	        InitTree(); // Initialize trees

	        /* code_buf[1..16] saves eight units of code,
	           and code_buf[0] works as eight flags,
	           "1" representing that the unit is an unencoded ubyte (1 byte),
	           "0" a position-and-length pair (2 bytes).
	           Thus, eight units require at most 16 bytes of code. */
	        code_buf[0] = 0;
	        code_buf_ptr = 1;
	        mask = 1;

	        s = 0;  r = LzssParameters.N - LzssParameters.F;

	        // Clear the buffer with any character that will appear often.
	        for (i = s; i < r; i++)
		        ringBuf[i] = LzssParameters.BUFF_INIT;

	        // Read F bytes into the last F bytes of the buffer
	        for (len = 0; len < LzssParameters.F && inputPos < input.Length; len++)
		        ringBuf[r + len] = input[inputPos++];

	        if (len == 0) // Text of size zero
		        return null;

	        /* Insert the F strings,
	           each of which begins with one or more 'space' characters.
	           Note	the order in which these strings are inserted.
	           This way, degenerate trees will be less likely to occur. */
	        for (i = 1; i <= LzssParameters.F; i++)
		        InsertNode(r - i);

	        /* Finally, insert the whole string just read.
	           The variables matchLength and matchPosition are set. */
	        InsertNode(r);

	        do {
		        // matchLength may be spuriously long near the end of text.
		        if (matchLength > len)
			        matchLength = len;

		        if (matchLength < LzssParameters.THRESHOLD) {
			        // Not long enough match. Send one byte.
			        matchLength = 1;
			        code_buf[0] |= mask; // 'send one byte' flag
			        code_buf[code_buf_ptr++] = ringBuf[r];  // Send uncoded.
		        } else {
			        // Send position and length pair. Note matchLength >= THRESHOLD.
			        code_buf[code_buf_ptr++] = (byte) matchPosition;
			        code_buf[code_buf_ptr++] = (byte)
				        (((matchPosition >> 4) & 0xf0)
			          | (matchLength - LzssParameters.THRESHOLD));
		        }

		        if ((mask <<= 1) == 0) { // Dropped high bit -> Buffer is full
                    for (i = 0; i < code_buf_ptr; i++)
                    {
                        app.Add(code_buf[i]);
                    }

			        code_buf[0] = 0;
			        code_buf_ptr = 1;
			        mask = 1;
		        }

		        last_matchLength = matchLength;
		        for (i = 0; i < last_matchLength && inputPos < input.Length; i++) {
			        // Delete old strings and read new bytes
			        DeleteNode(s);
			        ringBuf[s] = input[inputPos++];

			        /* If the position is near the end of buffer,
			         * extend the buffer to make string comparison easier. */
			        if (s < LzssParameters.F - 1)
				        ringBuf[s + LzssParameters.N] = input[inputPos-1];

			        // Since this is a ring buffer, increment the position modulo N.
			        s = (s + 1) % LzssParameters.N;  r = (r + 1) % LzssParameters.N;
			        InsertNode(r);	/* Register the string in ringBuf[r..r+F-1] */
		        }

		        // After the end of text, no need to read, but buffer may not be empty
		        while (i++ < last_matchLength) {
			        DeleteNode(s);
			        s = (s + 1) % LzssParameters.N;  r = (r + 1) % LzssParameters.N;
			        if (--len != 0)
				        InsertNode(r);
		        }
	        } while (len > 0);	/* until length of string to be processed is zero */

	        if (code_buf_ptr > 1) // Send remaining code.
            {
                for (i = 0; i < code_buf_ptr; i++)
                {
		            app.Add(code_buf[i]);
                }
            }

	        return app.ToArray();
        }
    }

    class LzssDecoder
    {
        public byte[] Decode(byte[] input)
        {
            List<byte> output = new List<byte>();
            byte[] ringBuf = new byte[LzssParameters.N];
            int inputPos = 0, ringBufPos = LzssParameters.N - LzssParameters.F;

            ushort flags = 0;

            // Clear ringBuf with a character that will appear often
            for (int i = 0; i < LzssParameters.N - LzssParameters.F; i++)
                ringBuf[i] = LzssParameters.BUFF_INIT;

            while (inputPos < input.Length)
            {
                // Use 16 bits cleverly to count to 8.
                // (After 8 shifts, the high bits will be cleared).
                if ((flags & 0xFF00) == 0)
                    flags = (ushort)(input[inputPos++] | 0x8000);

                if ((flags & 1) == 1)
                {
                    // Copy data literally from input
                    byte c = input[inputPos++];
                    output.Add(c);
                    ringBuf[ringBufPos++ % LzssParameters.N] = c;
                }
                else
                {
                    // Copy data from the ring buffer (previous data).
                    int index = ((input[inputPos + 1] & 0xF0) << 4) | input[inputPos];
                    int count = (input[inputPos + 1] & 0x0F) + LzssParameters.THRESHOLD;
                    inputPos += 2;

                    for (int i = 0; i < count; i++)
                    {
                        byte c = ringBuf[(index + i) % LzssParameters.N];
                        output.Add(c);
                        ringBuf[ringBufPos++ % LzssParameters.N] = c;
                    }
                }

                // Advance flags & count bits
                flags >>= 1;
            }

            return output.ToArray();
        }
    }

    public static class Lz
    {
        /// <summary>
        /// Unpack Amusement Vision LZ archive
        /// </summary>
        /// <param name="inputStream"></param>
        /// <param name="outputStream"></param>
        /// <param name="game"></param>
        public static void UnpackAvLz(Stream inputStream, Stream outputStream, AvGame game)
        {
            if (inputStream == null)
                throw new ArgumentNullException("inputStream");
            if (outputStream == null)
                throw new ArgumentNullException("outputStream");
            if (!Enum.IsDefined(typeof(AvGame), game))
                throw new ArgumentOutOfRangeException("game");

            EndianBinaryReader inputBinaryStream = new EndianBinaryReader(EndianBitConverter.Little, inputStream);

            // Read file header
            int headerSizeField = inputBinaryStream.ReadInt32();
            int uncompressedSize = inputBinaryStream.ReadInt32();
			int compressedSize = headerSizeField;

			// OK, let's get sophisticated
			// It looks like some AX files might be in GX format. 
			// So what we can do it compare the actual file length
			// and compare it to what's in the file. Depending on
			// the value, do the thing.
			if (game == AvGame.FZeroGX || game == AvGame.FZeroAX)
            {
				// If size in file and size in header are the same, we subtract 8 bytes
				// from the header size. If they
				int fileLength = (int)inputBinaryStream.BaseStream.Length;
				bool isMatching = headerSizeField == fileLength;
				// ... and if it isn't, ensure it is exactly 8 less. Otherwise we may
				// be dealing with a different kind of file.
				bool isMinus8 = headerSizeField == fileLength - 8;

				if (isMatching)
                {
					compressedSize -= 8;
                }
                else if (!isMinus8)
                {
					var msg = "You are not dealing with an F-Zero AX/GX LZ file!";
					throw new InvalidLzFileException(msg);
				}
			}
			// If an F-Zero game, do not hack count
			//int compressedSize = headerSizeField;
			//if (game != AvGame.FZeroGX)// && game != AvGame.FZeroAX)
            //    compressedSize -= 8; // SMB counts the 8 bytes of header in the compressed size field

            // Check that the size of the input matches the expected value
            if (compressedSize + 8 != inputStream.Length)
            {
                throw new InvalidLzFileException("Invalid .lz file, inputSize does not match actual input size.");
            }

            // Read and uncompress LZSS data
            byte[] compressedData = inputBinaryStream.ReadBytesOrThrow(compressedSize);

            LzssDecoder decoder = new LzssDecoder();
            byte[] uncompressedData = decoder.Decode(compressedData);
            if (uncompressedData.Length != uncompressedSize)
            {
                throw new InvalidLzFileException("Invalid .lz file, outputSize does not match actual output size.");
            }
            
            // Write uncompressed data to output stream
            outputStream.Write(uncompressedData, 0, uncompressedData.Length);
        }

        public static void PackAvLz(Stream inputStream, Stream outputStream, AvGame game)
        {
            if (inputStream == null)
                throw new ArgumentNullException("inputStream");
            if (outputStream == null)
                throw new ArgumentNullException("outputStream");
            if (!Enum.IsDefined(typeof(AvGame), game))
                throw new ArgumentOutOfRangeException($"{nameof(game)}, ({game})");

            // Read the input data and compress with LZSS
            byte[] uncompressedData = StreamUtil.ReadFully(inputStream);

            LzssEncoder encoder = new LzssEncoder();
            byte[] compressedData = encoder.Encode(uncompressedData);

            // Write file header and data
            int headerSizeField = compressedData.Length;
			if (game != AvGame.FZeroGX)
			{
				// SMB counts the 8 bytes of header in the compressed size field
				// F-Zero AX as well
				headerSizeField += 8; 
			}

            EndianBinaryWriter outputBinaryWriter = new EndianBinaryWriter(EndianBitConverter.Little, outputStream);
            outputBinaryWriter.Write(headerSizeField);
            outputBinaryWriter.Write(uncompressedData.Length);
            outputBinaryWriter.Write(compressedData);
        }
    }
}
