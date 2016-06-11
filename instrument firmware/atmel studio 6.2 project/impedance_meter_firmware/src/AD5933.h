/*
 * AD5933.h
 *
 * Created: 11.6.2016 15:42:00
 *  Author: user
 */ 


#ifndef AD5933_H_
#define AD5933_H_

#define TWI_AD5933_BUS_ADDR 0x50 //!< AD5933 TWI slave bus address

extern void AD5933_init(void);
extern bool AD5933_reg_write(uint8_t reg_addr, uint8_t reg_data);
extern bool AD5933_set_address_pointer(uint8_t reg_addr);
extern bool AD5933_block_write(uint8_t num_bytes, uint8_t* reg_data);
extern bool AD5933_reg_read(uint8_t reg_addr, uint8_t* reg_data);
extern bool AD5933_block_read(uint8_t num_bytes, uint8_t* reg_data);

#endif /* AD5933_H_ */