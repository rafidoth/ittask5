import { Card, Image, Text, Group, Badge, ActionIcon, Slider } from '@mantine/core';
import { IconThumbUpFilled, IconPlayerPlayFilled, IconPlayerPauseFilled } from '@tabler/icons-react';
import type { Song } from './types';
import { useState, useRef, useEffect } from 'react';
import { Howl } from 'howler';
import { useSeed, useCurrentlyPlayingId, useParameterActions } from '~/songs/parametersStore';

type SongCardProps = {
    song: Song;
};

export default function SongCard({ song }: SongCardProps) {
    const seed = useSeed();
    const currentlyPlayingId = useCurrentlyPlayingId();
    const { setCurrentlyPlayingId } = useParameterActions();
    const [isPlaying, setIsPlaying] = useState(false);
    const [progress, setProgress] = useState(0);
    const [duration, setDuration] = useState(0);
    const howlRef = useRef<Howl | null>(null);
    const animationRef = useRef<number | null>(null);

    const updateProgress = () => {
        if (howlRef.current && howlRef.current.playing()) {
            setProgress(howlRef.current.seek() as number);
            animationRef.current = requestAnimationFrame(updateProgress);
        }
    };

    const formatTime = (secs: number) => {
        const minutes = Math.floor(secs / 60);
        const seconds = Math.floor(secs % 60);
        return `${minutes}:${seconds < 10 ? '0' : ''}${seconds}`;
    };

    const handleSeek = (value: number) => {
        if (howlRef.current) {
            howlRef.current.seek(value);
            setProgress(value);
        }
    };

    useEffect(() => {
        if (currentlyPlayingId !== song.id && isPlaying) {
            howlRef.current?.pause();
            setIsPlaying(false);
        }
    }, [currentlyPlayingId, isPlaying, song.id]);

    const initHowl = () => {
        if (!howlRef.current) {
            howlRef.current = new Howl({
                src: [`${import.meta.env.VITE_BASE_URL}/api/songs/${song.id}/audio?seed=${seed}`],
                format: ['wav'],
                html5: true,
                onload: () => {
                    if (howlRef.current) setDuration(howlRef.current.duration());
                },
                onplay: () => {
                    animationRef.current = requestAnimationFrame(updateProgress);
                },
                onpause: () => {
                    if (animationRef.current) cancelAnimationFrame(animationRef.current);
                },
                onend: () => {
                    setIsPlaying(false);
                    setProgress(0);
                    if (animationRef.current) cancelAnimationFrame(animationRef.current);
                    setCurrentlyPlayingId(null);
                }
            });
        }
    };

    useEffect(() => {
        if (howlRef.current) {
            howlRef.current.unload();
            howlRef.current = null;
        }
        setIsPlaying(false);
        setProgress(0);
        setDuration(0);

        return () => {
            if (animationRef.current) cancelAnimationFrame(animationRef.current);
            if (howlRef.current) {
                howlRef.current.unload();
            }
        };
    }, [song.id, seed]);

    const togglePlay = () => {
        initHowl();
        if (howlRef.current) {
            if (isPlaying) {
                howlRef.current.pause();
                setCurrentlyPlayingId(null);
            } else {
                howlRef.current.play();
                setCurrentlyPlayingId(song.id);
            }
            setIsPlaying(!isPlaying);
        }
    };

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
            </Group>

            <Group mt="md" gap="sm" align="center" wrap="nowrap">
                <ActionIcon radius="xl" color="blue" variant="filled" size="md" onClick={togglePlay}>
                    {isPlaying ? <IconPlayerPauseFilled size={14} /> : <IconPlayerPlayFilled size={14} />}
                </ActionIcon>
                <Slider
                    value={progress}
                    onChange={handleSeek}
                    max={duration || 100}
                    color="blue"
                    size="sm"
                    style={{ flex: 1 }}
                    label={formatTime(progress)}
                />
                <Text size="xs" c="dimmed" style={{ whiteSpace: 'nowrap' }}>
                    {formatTime(progress)} / {duration ? formatTime(duration) : '0:00'}
                </Text>
            </Group>
        </Card>
    );
}
