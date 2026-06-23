import { Overlay, Stack, Text, Progress } from '@mantine/core';

export default function Loading() {
    return (
        <Overlay blur={2} center fixed zIndex={1000}>
            <Stack align="center" gap="md" w={300}>
                <Text c="white" fw={600} size="xl">Loading songs...</Text>
                <Progress value={100} animated w="100%" size="sm" radius="xl" color="blue" />
            </Stack>
        </Overlay>
    );
}
