library IEEE;
use IEEE.STD_LOGIC_1164.ALL;
use IEEE.STD_LOGIC_ARITH.ALL;
use IEEE.std_logic_unsigned.all;

entity spi_receiver is
	port(
		clk : in std_logic;
		sck : in std_logic;
		data : in std_logic;
		cs_n : in std_logic;
		irq : out std_logic;
		parallel_out : out std_logic_vector(7 downto 0)
	);
end spi_receiver;

architecture logic of spi_receiver is

	signal sck_prev : std_logic := '0';
	signal cs_prev : std_logic := '0';
	signal shift_reg : std_logic_vector(7 downto 0) := (others => '0');
	signal irq_delay : std_logic := '1';

begin

	sck_edge_detect:process(clk)
	begin
		if (clk'event and clk = '1') then
			if ((sck /= sck_prev) and (sck = '1')) then
				if (cs_n = '0') then
					shift_reg <= shift_reg(6 downto 0) & data;
				end if;
			end if;
			sck_prev <= sck;
		end if;
	end process;
	
	cs_edge_detect:process(clk)
	begin
		if (clk'event and clk = '1') then
			if (irq_delay = '0') then
				irq <= '0';
			else
				irq_delay <= '0';
			end if;
			if ((cs_n /= cs_prev) and (cs_n = '1')) then
				parallel_out <= shift_reg;
				irq <= '1';
				irq_delay <= '1';
			end if;
			cs_prev <= cs_n;
		end if;
	end process;
	
end logic;