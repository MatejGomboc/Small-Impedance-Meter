library IEEE;
use IEEE.STD_LOGIC_1164.ALL;
use IEEE.STD_LOGIC_ARITH.ALL;
use IEEE.std_logic_unsigned.all;

entity clk_divider is
	port(
		clk : in std_logic;
		setting : in std_logic_vector(7 downto 0);
		wrt : in std_logic;
		clk_out : out std_logic
	);
end clk_divider;

architecture logic of clk_divider is
	signal settings_reg : std_logic_vector(7 downto 0) := (others => '0');
	signal counter : std_logic_vector(7 downto 0) := (others => '0');
	signal output : std_logic := '1';
begin

	clk_out <= clk when settings_reg = 0 else output;

	divider:process(clk)
	begin
		if (clk'event and (clk = '1')) then
		
			if (wrt = '1') then
				settings_reg <= setting;
				counter <= (others => '0');
			end if;
			
			if (counter = (settings_reg - 1)) then
				output <= not output;
				counter <= (others => '0');
			else
				if (wrt = '0') then
					counter <= counter + 1;
				end if;
			end if;
			
		end if;
	end process;
	
end logic;