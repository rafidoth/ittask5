import { useState, useEffect } from 'react';
import { Table, Box, Group, Center, Pagination, LoadingOverlay } from '@mantine/core';
import type { Song, GenerationResponse } from './types';
import React from 'react';
import SongsTableRow from './SongsTableRow';
import { useQuery, useQueryClient } from "@tanstack/react-query";
import { getSongs } from "~/api";
import { useLanguage, useLikes, useSeed } from "~/songs/parametersStore";



export default function SongsTable() {
  const language = useLanguage();
  const likes = useLikes();
  const seed = useSeed();
  const [page, setPage] = useState(1);

  useEffect(() => {
    setPage(1);
  }, [language, likes, seed]);

  const queryClient = useQueryClient();

  const { data: responseData, isPending, isFetching, isSuccess } = useQuery<GenerationResponse>({
    queryKey: ["songs", seed, language, likes, page],
    queryFn: () => getSongs(seed, language, likes, page),
    retry: false,
    staleTime: 60000,
  });

  useEffect(() => {
    if (isSuccess) {
      if (page < 1000) {
        queryClient.prefetchQuery({
          queryKey: ["songs", seed, language, likes, page + 1],
          queryFn: () => getSongs(seed, language, likes, page + 1),
          staleTime: 60000,
        });
      }

      if (page > 1) {
        queryClient.prefetchQuery({
          queryKey: ["songs", seed, language, likes, page - 1],
          queryFn: () => getSongs(seed, language, likes, page - 1),
          staleTime: 60000,
        });
      }
    }
  }, [page, isSuccess, seed, language, likes, queryClient]);

  const data = responseData?.songs || [];
  const isLoading = isPending || isFetching;

  const [expandedId, setExpandedId] = useState<string | null>(null);
  const toggleRow = (id: string) => {
    setExpandedId(prev => prev === id ? null : id);
  };

  if (!isLoading && (!data || data.length === 0)) {
    return <p>No songs available</p>;
  }

  const rows = data.map((element) => (
    <SongsTableRow
      key={element.id}
      song={element}
      page={page}
      seed={seed}
      isExpanded={expandedId === element.id}
      onToggle={() => toggleRow(element.id)}
    />
  ));

  return (
    <Box pos="relative" w="100%" mt="md">
      <LoadingOverlay visible={isLoading || false} zIndex={1000} overlayProps={{ radius: "sm", blur: 2 }} />
      <Center>
        <Table highlightOnHover style={{ marginTop: '16px' }} fz="md">
          <Table.Thead style={{ borderBottom: '2px solid var(--mantine-color-dark-8)' }} >
            <Table.Tr fw={1000}>
              <Table.Th style={{ width: 40 }}></Table.Th>
              <Table.Th>#</Table.Th>
              <Table.Th>Title</Table.Th>
              <Table.Th>Artist</Table.Th>
              <Table.Th>Album</Table.Th>
              <Table.Th>Genre</Table.Th>
            </Table.Tr>
          </Table.Thead>
          <Table.Tbody>
            {rows}
          </Table.Tbody>
        </Table>
      </Center>
      <Group justify="center" mt="xl">
        <Pagination
          value={page}
          onChange={setPage}
          total={1000}
          siblings={1}
          boundaries={0}
        />
      </Group>
    </Box>
  );
}