/*
 * AD5933.h
 *
 * Created: 11.6.2016 15:42:00
 *  Author: user
 */ 


#ifndef AD5933_H_
#define AD5933_H_

#define TWI_AD5933_BUS_ADDR 0x50 //!< AD5933 TWI slave bus address

//!< AD5933 registers address definitions
#define AD5933_ADDR_CTRL_HIGH 0x80 			//RW 2 bytes
#define AD5933_ADDR_CTRL_LOW 0x81			//RW 2 bytes
#define AD5933_ADDR_FREQ_HIGH 0x82			//RW 3 bytes
#define AD5933_ADDR_FREQ_MID 0x83
#define AD5933_ADDR_FREQ_LOW 0x84
#define AD5933_ADDR_FREQ_INC_HIGH 0x85		//RW 3 bytes
#define AD5933_ADDR_FREQ_INC_MID 0x86
#define AD5933_ADDR_FREQ_INC_LOW 0x87
#define AD5933_ADDR_NUM_INC_HIGH 0x88		//RW 2 bytes, 9 bit
#define AD5933_ADDR_NUM_INC_LOW 0x89
#define AD5933_ADDR_NUM_SETTLE_HIGH 0x8A	//RW 2 bytes
#define AD5933_ADDR_NUM_SETTLE_LOW 0x8B
#define AD5933_ADDR_STATUS 0x8F				//R 1 byte
#define AD5933_ADDR_TEMP_HIGH 0x92			//R 2 bytes
#define AD5933_ADDR_TEMP_LOW 0x93
#define AD5933_ADDR_REAL_HIGH 0x94			//R 2 bytes
#define AD5933_ADDR_REAL_LOW 0x95
#define AD5933_ADDR_IMAG_HIGH 0x96			//R 2 bytes
#define AD5933_ADDR_IMAG_LOW 0x97

//!< AD5933 control commands
#define AD5933_CMD_INIT_WITH_START_FREQ 0x01
#define AD5933_CMD_START_FREQ_SWEEP 0x02
#define AD5933_CMD_INCREMENT_FREQ 0x03
#define AD5933_CMD_REPEAT_FREQ 0x04
#define AD5933_CMD_MEASURE_TEMP 0x09
#define AD5933_CMD_POWERDOWN 0x0A
#define AD5933_CMD_STANDBY 0x0B
#define AD5933_CMD_RESET 0x01

extern void switch_to_calibrate(void);
extern void switch_to_measure(void);

extern void AD5933_init(void);

extern bool AD5933_reg_write(uint8_t reg_addr, uint8_t reg_data);
extern bool AD5933_set_address_pointer(uint8_t reg_addr);
extern bool AD5933_block_write(uint8_t num_bytes, uint8_t* reg_data);
extern bool AD5933_reg_read(uint8_t reg_addr, uint8_t* reg_data);
extern bool AD5933_block_read(uint8_t num_bytes, uint8_t* reg_data);

extern bool AD5933_ctrl_init_with_start_freq(void);
extern bool AD5933_ctrl_start_freq_sweep(void);
extern bool AD5933_ctrl_increment_freq(void);
extern bool AD5933_ctrl_repeat_freq(void);
//extern bool AD5933_ctrl_measure_temp(void);
extern bool AD5933_ctrl_powedown(void);
extern bool AD5933_ctrl_standby(void);
//extern bool AD5933_ctrl_select_Vout_range(uint8_t setting);
//extern bool AD5933_ctrl_set_PGA_gain(bool setting);
extern bool AD5933_ctrl_reset_part(void);
//extern bool AD5933_ctrl_select_mclk(bool setting);

#endif /* AD5933_H_ */