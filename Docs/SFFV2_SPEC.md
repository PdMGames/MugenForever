## Introduction ##

This document is a specification for the SFF 2.0 format, also known as SFFv2.

SFFv2 file format is designed for fast loading, fast decompression and low runtime memory consumption.
In 2.00, only 5 and 8-bit paletted sprites are supported.
SFFv2 is an open file format. Elecbyte grants permission to implement the spec freely in any application.

### Terminology

- **ldata**

	literal data.  Must be loaded verbatim.  In M.U.G.E.N, sprite data in the
	ldata block is loaded verbatim into memory, and decompressed as necessary on-the-fly.

- **tdata**
	
	translate data.  Must be translated (e.g. decompressed) during load.

## SFF 2.00 ##

### SFF header 2.00

All values are little-endian.

<table>
	<thead>
		<tr>
			<th colspan="5">512 bytes</th>
		</tr>
		<tr>
			<th>dec</th>
			<th>hex</th>
			<th>size</th>
			<th>value</th>
			<th>desc</th>
		</tr>	
	</thead>
	<tbody>
		<tr>
			<td>0</td>
			<td>0</td>
			<td>12</td>
			<td>ElecbyteSpr\0</td>
			<td>signature</td>
		</tr>
		<tr>
			<td>12</td>
			<td>C</td>
			<td>1</td>
			<td>0</td>
			<td>verlo3</td>
		</tr>
		<tr>
			<td>13</td>
			<td>D</td>
			<td>1</td>
			<td>0</td>
			<td>verlo2</td>
		</tr>
		<tr>
			<td>14</td>
			<td>E</td>
			<td>1</td>
			<td>0</td>
			<td>verlo1</td>
		</tr>
		<tr>
			<td>15</td>
			<td>F</td>
			<td>1</td>
			<td>2</td>
			<td>verhi</td>
		</tr>
		<tr>
			<td>16</td>
			<td>10</td>
			<td>4</td>
			<td>0</td>
			<td>reserved</td>
		</tr>
		<tr>
			<td>20</td>
			<td>14</td>
			<td>4</td>
			<td>0</td>
			<td>reserved</td>
		</tr>
		<tr>
			<td>24</td>
			<td>18</td>
			<td>1</td>
			<td>0</td>
			<td>compatverlo3</td>
		</tr>
		<tr>
			<td>25</td>
			<td>19</td>
			<td>1</td>
			<td>0</td>
			<td>compatverlo1</td>
		</tr>
		<tr>
			<td>26</td>
			<td>1A</td>
			<td>1</td>
			<td>0</td>
			<td>compatverlo2</td>
		</tr>
		<tr>
			<td>27</td>
			<td>1B</td>
			<td>1</td>
			<td>2</td>
			<td>compatverhi</td>
		</tr>
		<tr>
			<td>28</td>
			<td>1C</td>
			<td>1</td>
			<td>0</td>
			<td>reserved</td>
		</tr>
		<tr>
			<td>36</td>
			<td>24</td>
			<td>4</td>
			<td>int32</td>
			<td>offset where first sprite node header data is located</td>
		</tr>
		<tr>
			<td>40</td>
			<td>28</td>
			<td>4</td>
			<td>int32</td>
			<td>Total number of sprites</td>
		</tr>
		<tr>
			<td>44</td>
			<td>2C</td>
			<td>4</td>
			<td>int32</td>
			<td>offset where first palette node header data is located</td>
		</tr>
		<tr>
			<td>48</td>
			<td>30</td>
			<td>4</td>
			<td>int32</td>
			<td>Total number of palettes</td>
		</tr>
		<tr>
			<td>52</td>
			<td>34</td>
			<td>4</td>
			<td>int32</td>
			<td>ldata offset</td>
		</tr>
		<tr>
			<td>56</td>
			<td>38</td>
			<td>4</td>
			<td>int32</td>
			<td>ldata length</td>
		</tr>
		<tr>
			<td>60</td>
			<td>3C</td>
			<td>4</td>
			<td>int32</td>
			<td>tdata offset</td>
		</tr>
		<tr>
			<td>64</td>
			<td>40</td>
			<td>4</td>
			<td>0</td>
			<td>reserved</td>
		</tr>
		<tr>
			<td>68</td>
			<td>44</td>
			<td>4</td>
			<td>0</td>
			<td>reserved</td>
		</tr>
		<tr>
			<td>76</td>
			<td>4C</td>
			<td>436</td>
			<td>string</td>
			<td>unused; or comments</td>
		</tr>
	</tbody>
</table>

compatver

	Minimum version of loader needed to read this SFF: 2.00


### Sprite node header 2.00 ###

<table>
	<thead>
		<tr>
			<th colspan="5">28 bytes</th>
		</tr>
		<tr>
			<th>dec</th>
			<th>hex</th>
			<th>size</th>
			<th>value</th>
			<th>desc</th>
		</tr>	
	</thead>
	<tbody>
		<tr>
			<td>0</td>
			<td>0</td>
			<td>2</td>
			<td>int16</td>
			<td>groupno</td>
		</tr>
		<tr>
			<td>2</td>
			<td>2</td>
			<td>2</td>
			<td>int16</td>
			<td>itemno</td>
		</tr>	
		<tr>
			<td>4</td>
			<td>4</td>
			<td>2</td>
			<td>int16</td>
			<td>width</td>
		</tr>	
		<tr>
			<td>6</td>
			<td>6</td>
			<td>2</td>
			<td>int16</td>
			<td>height</td>
		</tr>	
		<tr>
			<td>8</td>
			<td>8</td>
			<td>2</td>
			<td>int16</td>
			<td>axisx</td>
		</tr>	
		<tr>
			<td>10</td>
			<td>A</td>
			<td>2</td>
			<td>int16</td>
			<td>axisy</td>
		</tr>	
		<tr>
			<td>12</td>
			<td>C</td>
			<td>2</td>
			<td>int16</td>
			<td>Index number of the linked sprite (if linked)</td>
		</tr>	
		<tr>
			<td>14</td>
			<td>E</td>
			<td>1</td>
			<td>int</td>
			<td>fmt</td>
		</tr>	
		<tr>
			<td>15</td>
			<td>F</td>
			<td>1</td>
			<td>int</td>
			<td>coldepth</td>
		</tr>	
		<tr>
			<td>16</td>
			<td>10</td>
			<td>4</td>
			<td>int32</td>
			<td>offset into ldata or tdata</td>
		</tr>	
		<tr>
			<td>20</td>
			<td>14</td>
			<td>4</td>
			<td>int32</td>
			<td>Sprite data length (0: linked)</td>
		</tr>	
		<tr>
			<td>24</td>
			<td>18</td>
			<td>2</td>
			<td>int16</td>
			<td>palette index</td>
		</tr>	
		<tr>
			<td>26</td>
			<td>1A</td>
			<td>2</td>
			<td>int16</td>
			<td>flags</td>
		</tr>	
	</tbody>
<table>

**fmt

<table>
	<thead>
		<tr>
			<th>code</th>
			<th>type</th>
		</tr>
	</thead>
	<tbody>
		<tr>
			<td>0</td>
			<td>raw</td>
		</tr>
		<tr>
			<td>1</td>
			<td>invalid (no use)</td>
		</tr>
		<tr>
			<td>2</td>
			<td>RLE8</td>
		</tr>
		<tr>
			<td>3</td>
			<td>RLE5</td>
		</tr>
		<tr>
			<td>4</td>
			<td>LZ5</td>
		</tr>
	</tbody>
</table>

**flags
 
 	0    unset: literal (use ldata); set: translate (use tdata; decompress on load)
 	1-15 unused


### Pal node header 2.00

16 bytes
 dec  hex  size   meaning
 0    0     2  groupno
 2    2     2  itemno
 4    4     2  numcols
 6    6     2  Index number of the linked palette (if linked)
 8    8     4  Offset into ldata
 12    C     4  Palette data length (0: linked)

Palette data is stored in 4 byte chunks per color. The first 3 bytes correspond to 8-bit values for RGB color, and the last byte is unused (set to 0).

### Compression Formats

Compression formats are consistent across SFF versions. The first 4 bytes of each compressed block is an integer representing the length of the data after decompression.

**RLE8

Simple run-length encoding for 8-bit-per-pixel bitmaps.
Any byte with bits 6 and 7 set to 1 and 0 respectively is an RLE control packet. All other bytes represent the value of the pixel.
RLE packet (1 byte)
       bits 0-5: run length
       bits 6-7: run marker (01)

Pseudocode to decode an RLE8 chunk:
	
	one_byte = read(1 byte)
       	if (one_byte & 0xC0) == 0x40, then
         	color = read(1 byte)
         	for run_count from 0 to (val & 0x3F) - 1, do
           	output(color)
       	else
         	output(one_byte)

**RLE5

RLE5 is a run-length encoding used for the compression of 5-bit-per-pixel bitmaps. RLE5 is a hybrid of two run-length encoding algorithms. The first allows encoding of long runs of color 0 pixels, the second is a 3-bit-RL/5-bit-color run-length algorithm.

RLE5 packet (2 bytes)
       byte 0         : run length
       byte 1 bit 7   : color bit; .2-.7
       byte 1 bits 0-6: data length

Pseudocode to decode an RLE5 chunk:

<pre>       
	RLE5packet = read(2 bytes)
	
       	if RLE5packet.color_bit is 1, then
         	color = read(1 byte)
       	else
		color = 0
		
       	for run_count from 0 to RLE5packet.run_length, do
		output(color)
       		// Decode 3RL/5VAL
       		for bytes_processed from 0 to RLE5packet.data_length - 1, do
         		one_byte = read(1 byte)
         		color = one_byte & 0x1F
         		run_length = one_byte >> 5
         		for run_count from 0 to run_length, do
           			output(color)
</pre>

**LZ5

LZ5 is a run-length encoding used for the compression of 5-bit-per-pixel bitmaps. It is more efficient than RLE5 but is more complex to encode.
