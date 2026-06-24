import { Box, Paper, Select, Flex, Text, RollingNumber, Slider, Stack, SegmentedControl } from "@mantine/core";
import { IconArrowsShuffle, IconChevronDown, IconGrid4x4, IconLayoutGrid, IconTable } from "@tabler/icons-react";
import { useState, useEffect } from "react";
import { useLanguage, useLikes, useParameterActions, useSeed, useViewMode } from "./parametersStore";
import type { ViewMode } from "./types";
import { ViewToggle } from "./ViewToggler";

export default function ParameterToolbar() {
    const language = useLanguage();
    const likes = useLikes();
    const seed = useSeed();
    const viewMode = useViewMode();
    const actions = useParameterActions();
    const [localLikes, setLocalLikes] = useState(likes);

    useEffect(() => {
        setLocalLikes(likes);
    }, [likes]);

    return (
        <Paper >
            <Flex justify={"space-between"} bg="gray.0" bdrs={"md"} align={"center"} p="md">
                <Flex p="md" gap="md">
                    <Box style={{ background: "white" }} bd={"1px solid gray.3"} px="md" py="xs" bdrs={"sm"} w={200}>
                        <Text p={0} m={0} size="xs" c="dimmed" fw={"bold"}>Language</Text>
                        <Select
                            rightSection={
                                <IconChevronDown size={18}
                                    style={{ cursor: "pointer", color: "black" }}
                                    onClick={() => console.log("Shuffle languages")}
                                />}
                            leftSectionProps={{ style: { padding: "0px" } }}
                            variant="unstyled"
                            data={[
                                { value: "en", label: "English (US)" },
                                { value: "es", label: "Spanish" },
                            ]}
                            value={language}
                            onChange={(value) => actions.setLanguage(value || "en")}
                        />
                    </Box>

                    <Box style={{ background: "white" }} bd={"1px solid gray.3"} px="md" py="xs" bdrs={"sm"} w={200}>
                        <Text p={0} mb={4} size="xs" c="dimmed" fw={"bold"}>Seed</Text>
                        <Flex justify={"space-between"} align={"center"}>
                            <RollingNumber value={seed} />
                            <IconArrowsShuffle
                                size={18} style={{ cursor: "pointer" }}
                                onClick={() => actions.setSeed(Math.floor(Math.random() * 10000))}
                            />
                        </Flex>
                    </Box>
                    <Stack justify="center" px="md" py="xs" bdrs={"sm"} w={200}>
                        <Text p={0} size="xs" c="dimmed" fw={"bold"}>Likes</Text>
                        <Slider
                            color="blue"
                            min={0}
                            max={10}
                            step={0.1}
                            value={localLikes}
                            onChange={(value) => setLocalLikes(value)}
                            onChangeEnd={(value) => actions.setLikes(value)}
                            marks={Array.from({ length: 11 }, (_, i) => ({ value: i }))}
                        />
                    </Stack>
                </Flex>
                <ViewToggle />
            </Flex>
        </Paper>
    );
}


