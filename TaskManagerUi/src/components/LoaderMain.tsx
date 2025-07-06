import { Flex, Loader } from '@mantine/core';

export function LoaderMain() {

  return (
    <Flex
      justify="center"
      align="center"
      style={{ height: '100vh', width: '100vw' }}
    >
      <Loader color="blue" size="md" type="bars" />
    </Flex>);
}