library IEEE;
use IEEE.STD_LOGIC_1164.ALL;
use IEEE.STD_LOGIC_ARITH.ALL;
use IEEE.std_logic_unsigned.all;

entity clk_gen is
	port(
		clk : in std_logic;
		sck : in std_logic;
		data : in std_logic;
		cs_n : in std_logic;
		output : out std_logic
	);
end clk_gen;

architecture logic of clk_gen is

	component spi_receiver is
		port(
			clk : in std_logic;
			sck : in std_logic;
			data : in std_logic;
			cs_n : in std_logic;
			irq : out std_logic;
			parallel_out : out std_logic_vector(7 downto 0)
		);
	end component;

	component clk_divider is
		port(
			clk : in std_logic;
			setting : in std_logic_vector(7 downto 0);
			wrt : in std_logic;
			clk_out : out std_logic
		);
	end component;

	signal irq : std_logic := '0';
	signal setting : std_logic_vector(7 downto 0) := (others => '1');

begin

	spi_recvr:spi_receiver
   port map( 
				clk => clk,
				sck => sck,
				data => data,
				cs_n => cs_n,
				irq => irq,
				parallel_out => setting
			  );
	
	clk_div:clk_divider
   port map( 
				clk => clk,
				setting => setting,
				wrt => irq,
				clk_out => output
			  );

end logic;