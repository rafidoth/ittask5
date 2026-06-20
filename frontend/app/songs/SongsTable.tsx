import { useState } from 'react';
import { Table, Box, Group, ActionIcon, Center, Paper, Collapse } from '@mantine/core';
import { IconChevronDown } from '@tabler/icons-react';
import type { Song } from './types';
import React from 'react';

type SongsTableProps = {
  data: Song[];
};

export default function SongsTable({ data }: SongsTableProps) {

  const [expandedId, setExpandedId] = useState<string | null>(null);
  const toggleRow = (id: string) => {
    setExpandedId(prev => prev === id ? null : id);
  };

  if (!data || data.length === 0) {
    return <p>No songs available</p>;
  }
  const rows = data.map((element) => (

    <React.Fragment key={element.id}>
      <Table.Tr key={element.id} >
        <Table.Td>
          <ActionIcon variant="transparent" c="gray.6" onClick={() => toggleRow(element.id)}>
            <IconChevronDown />
          </ActionIcon>
        </Table.Td>
        <Table.Td>{element.id}</Table.Td>
        <Table.Td>{element.title}</Table.Td>
        <Table.Td>{element.artist}</Table.Td>
        <Table.Td>{element.album}</Table.Td>
        <Table.Td>{element.genre}</Table.Td>
      </Table.Tr>

      <Table.Tr key={element.id} >
        <Table.Td colSpan={6} style={{ padding: 0, border: 0 }}>
          <Collapse expanded={expandedId === element.id} transitionDuration={300} keepMounted={false}>
            <Paper style={{ width: "100%" }}>
              <Box p="md">
                TODO : display more details about the song, like lyrics, release date, etc.
              </Box>
            </Paper>
          </Collapse>
        </Table.Td>
      </Table.Tr>
    </React.Fragment>

  ));

  return (
    <Center>
      <Table striped highlightOnHover style={{ marginTop: '16px' }}>
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
        {rows}
      </Table>
    </Center>
  );
}