import { Flex, Image, Stack, Title, Text, ActionIcon, Slider, Badge, Tabs, Group, ScrollArea } from "@mantine/core";
import { IconPlayerPlayFilled, IconPlayerPauseFilled, IconVolume, IconThumbUpFilled } from "@tabler/icons-react";
import type { Song } from "./types";
import { useState, useRef, useEffect } from "react";
import { Howl } from 'howler';
import { useCurrentlyPlayingId, useParameterActions } from '~/songs/parametersStore';

export default function SongDetails({ song, seed }: { song: Song, seed: number }) {
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
                src: [`http://localhost:5017/api/songs/${song.id}/audio?seed=${seed}`],
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
        <Flex gap="xl" p="md" align="flex-start">
            <Stack align="center" gap="sm">
                <Image
                    src={song.coverImageBase64 ? `data:image/png;base64,${song.coverImageBase64}` : "https://placehold.co/200x200?text=No+Cover"}
                    alt="album cover"
                    w={200}
                    h={200}
                    radius="sm"
                />
                <Badge size="lg" radius="xl" color="blue" variant="filled" style={{ textTransform: 'none' }}>
                    <Group gap={6} align="center" wrap="nowrap">
                        <Text span size="sm" fw={700}>{song.likes ?? 0}</Text>
                        <IconThumbUpFilled size={14} />
                    </Group>
                </Badge>
            </Stack>
            <Stack gap={4} style={{ flex: 1 }}>
                <Title order={2} c="dark">{song.title || "Unknown Title"}</Title>
                <Group gap="sm" align="center">
                    <ActionIcon radius="xl" color="blue" variant="filled" size="md" onClick={togglePlay}>
                        {isPlaying ? <IconPlayerPauseFilled size={14} /> : <IconPlayerPlayFilled size={14} />}
                    </ActionIcon>
                    <Slider
                        value={progress}
                        onChange={handleSeek}
                        max={duration || 100}
                        w={150}
                        color="blue"
                        size="sm"
                        label={formatTime(progress)}
                    />
                    <Badge color="gray" variant="filled" radius="xl" style={{ textTransform: 'none' }}>
                        {formatTime(progress)} / {duration ? formatTime(duration) : '0:00'}
                    </Badge>
                </Group>

                <Text size="lg" c="dimmed" mt={4}>
                    from <Text span fw={700} c="dark">{song.album || "Unknown Album"}</Text> by <Text span fs="italic" fw={700} c="dark">{song.artist || "Unknown Artist"}</Text>
                </Text>
            </Stack>
        </Flex>
    );
}