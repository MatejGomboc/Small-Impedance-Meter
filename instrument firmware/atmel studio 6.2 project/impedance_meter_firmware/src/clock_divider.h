/*
 * clock_divider.h
 *
 * Created: 11.6.2016 11:37:04
 *  Author: user
 */ 


#ifndef CLOCK_DIVIDER_H_
#define CLOCK_DIVIDER_H_

#include "sysclk.h"

#define CLK_GEN_SPI_BAUDRATE  sysclk_get_cpu_hz()
#define CLK_GEN_TCXO_FREQ_HZ  16000000

extern void clk_gen_init(void);
extern void clk_gen_set(uint8_t setting);

#endif /* CLOCK_DIVIDER_H_ */