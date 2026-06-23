import { useState } from 'react';
import { Table, Box, Group, ActionIcon, Center, Paper, Collapse, Text, Pagination, LoadingOverlay } from '@mantine/core';
import { IconChevronDown } from '@tabler/icons-react';
import type { Song } from './types';
import React from 'react';
import SongDetails from './SongDetails';

type SongsTableProps = {
  data: Song[];
  page: number;
  onPageChange: (page: number) => void;
  isLoading?: boolean;
};

export default function SongsTable({ data, page, onPageChange, isLoading }: SongsTableProps) {
  const [expandedId, setExpandedId] = useState<string | null>(null);
  const toggleRow = (id: string) => {
    setExpandedId(prev => prev === id ? null : id);
  };

  if (!isLoading && (!data || data.length === 0)) {
    return <p>No songs available</p>;
  }

  const rows = data.map((element) => (
    <React.Fragment key={element.id}>
      <Table.Tr key={element.id} onClick={() => toggleRow(element.id)} style={{ cursor: 'pointer' }}>
        <Table.Td>
          <ActionIcon variant="transparent" c="gray.6">
            <IconChevronDown />
          </ActionIcon>
        </Table.Td>
        <Table.Td><Text fw={700}>{(page - 1) * 12 + element.id}</Text></Table.Td>
        <Table.Td>{element.title}</Table.Td>
        <Table.Td>{element.artist}</Table.Td>
        <Table.Td>{element.album == 'Single' ? <Text c="dimmed">Single</Text> : element.album}</Table.Td>
        <Table.Td>{element.genre}</Table.Td>
      </Table.Tr>

      <Table.Tr key={element.title} >
        <Table.Td colSpan={6} style={{ padding: 0, border: 0 }}>
          <Collapse expanded={expandedId === element.id} transitionDuration={300} keepMounted={false}>
            <Paper style={{ width: "100%" }}>
              <SongDetails song={element} />
            </Paper>
          </Collapse>
        </Table.Td>
      </Table.Tr>
    </React.Fragment>
  ));

  return (
    <Box pos="relative" w="100%" mt="md">
      <LoadingOverlay visible={isLoading || false} zIndex={1000} overlayProps={{ radius: "sm", blur: 2 }} />
      <Center>
        <Table highlightOnHover style={{ marginTop: '16px' }}>
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
          onChange={onPageChange}
          total={1000}
          siblings={1}
          boundaries={0}
        />
      </Group>
    </Box>
  );
}