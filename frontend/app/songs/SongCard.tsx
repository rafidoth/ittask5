import { Card, Image, Text, Group, Badge, ActionIcon } from '@mantine/core';
import { IconThumbUpFilled, IconPlayerPlayFilled } from '@tabler/icons-react';
import type { Song } from './types';

type SongCardProps = {
    song: Song;
};

export default function SongCard({ song }: SongCardProps) {
    return (
        <Card
            shadow="sm"
            padding="lg"
            radius="md"
            withBorder
            style={{ display: 'flex', flexDirection: 'column', height: '100%' }}
        >
            <Card.Section>
                <Image
                    src={song.coverImageBase64 ? `data:image/png;base64,${song.coverImageBase64}` : "https://placehold.co/400x400?text=No+Cover"}
                    height={300}
                    alt="album cover"
                />
            </Card.Section>

            <Group justify="space-between" mt="md" mb="xs" wrap="nowrap">
                <Text fw={700} truncate style={{ flex: 1 }} title={song.title || "Unknown Title"}>{song.title || "Unknown Title"}</Text>
                <Badge color="blue" variant="filled" style={{ textTransform: 'none' }}>
                    <Group gap={4}>
                        <Text span size="xs" fw={700}>{song.likes ?? 0}</Text>
                        <IconThumbUpFilled size={10} />
                    </Group>
                </Badge>
            </Group>

            <Text size="sm" c="dimmed" lineClamp={2} style={{ flex: 1, minHeight: 40 }}>
                from <Text span fw={700} c="dark">{song.album === 'Single' ? <Text span c="dimmed">Single</Text> : song.album || "Unknown Album"}</Text> by <Text span fs="italic" fw={700} c="dark">{song.artist || "Unknown Artist"}</Text>
            </Text>

            <Group justify="space-between" mt="md" align="flex-end">
                <Badge color="gray" variant="outline">{song.genre || "Unknown Genre"}</Badge>
                <ActionIcon variant="filled" color="blue" radius="xl">
                    <IconPlayerPlayFilled size={14} />
                </ActionIcon>
            </Group>
        </Card>
    );
}
