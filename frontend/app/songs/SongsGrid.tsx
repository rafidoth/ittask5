import { useState, useEffect } from 'react';
import { Box, SimpleGrid, Center, Loader, Text, Affix, Transition, ActionIcon } from '@mantine/core';
import { useIntersection, useWindowScroll } from '@mantine/hooks';
import { useQueries } from '@tanstack/react-query';
import { IconArrowUp } from '@tabler/icons-react';
import { getSongs } from '~/api';
import { useLanguage, useLikes, useSeed } from '~/songs/parametersStore';
import SongCard from './SongCard';

export default function SongsGrid() {
    const language = useLanguage();
    const likes = useLikes();
    const seed = useSeed();
    const [pagesToLoad, setPagesToLoad] = useState<number[]>([1]);

    useEffect(() => {
        setPagesToLoad([1]);
    }, [language, likes, seed]);

    const queryResults = useQueries({
        queries: pagesToLoad.map(page => ({
            queryKey: ["songs", seed, language, likes, page],
            queryFn: () => getSongs(seed, language, likes, page),
            staleTime: 60000,
        }))
    });

    const isFetchingAny = queryResults.some(q => q.isPending || q.isFetching);
    const data = queryResults.flatMap(q => q.data?.songs || []);

    const { ref, entry } = useIntersection({
        root: null,
        threshold: 0.1,
    });

    useEffect(() => {
        if (entry?.isIntersecting && !isFetchingAny) {
            setPagesToLoad(prev => [...prev, prev.length + 1]);
        }
    }, [entry?.isIntersecting, isFetchingAny]);

    const [scroll, scrollTo] = useWindowScroll();

    if (!isFetchingAny && data.length === 0) {
        return <Text mt="xl" ta="center">No songs available</Text>;
    }

    return (
        <Box pos="relative" w="100%" mt="md">
            <SimpleGrid cols={{ base: 1, sm: 2, lg: 3 }} spacing="md" verticalSpacing="md" mt="md">
                {data.map((song, index) => (
                    <SongCard key={`${song.id}-${index}`} song={song} />
                ))}
            </SimpleGrid>
            <Center ref={ref} mt="xl" mb="xl" style={{ minHeight: 50 }}>
                {isFetchingAny && <Loader size="sm" />}
            </Center>

            <Affix position={{ bottom: 20, right: 20 }}>
                <Transition transition="slide-up" mounted={scroll.y > 0}>
                    {(transitionStyles) => (
                        <ActionIcon
                            color="blue"
                            size="xl"
                            radius="xl"
                            variant="filled"
                            style={transitionStyles}
                            onClick={() => scrollTo({ y: 0 })}
                        >
                            <IconArrowUp style={{ width: '70%', height: '70%' }} />
                        </ActionIcon>
                    )}
                </Transition>
            </Affix>
        </Box>
    );
}
