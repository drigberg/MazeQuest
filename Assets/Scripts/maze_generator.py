import random
import sys
import typing as t

Point = t.Tuple[int, int]
Section = t.Tuple[Point, Point, Point, Point]


"""
All sections are assumed to be in the following format:
[
    (low_x, low_y),
    (high_x, low_y),
    (high_x, high_y),
    (low_x, high_y),
]
"""


def get_bounds(section: Section):
    return {
        "left": section[0][0],
        "right": section[1][0],
        "lower": section[0][1],
        "upper": section[2][1],
    }


def can_subdivide_section(section: Section) -> bool:
    """
    Section must have room for a wall in either direction: there must be an
    even number between the bounds
    """
    bounds = get_bounds(section)
    return (
        abs(bounds["right"] - bounds["left"]) >= 4
        and abs(bounds["upper"] - bounds["lower"]) >= 4)


def random_even_number_between_evens(low: int, high: int):
    return low + 2 * random.randint(1, abs(high - low) / 2 - 1)


def random_odd_number_between_evens(low: int, high: int):
    return low + 1 + 2 * random.randint(0, abs(high - low) / 2 - 1)


def divide_section(section: Section) -> t.Tuple[
        t.Sequence[Section],
        t.Sequence[Point]]:
    bounds = get_bounds(section)
    vertical_split = random_even_number_between_evens(
        bounds["left"], bounds["right"])
    horizontal_split = random_even_number_between_evens(
        bounds["lower"], bounds["upper"])
    new_sections = [
        [
            (bounds["left"], bounds["lower"]),
            (vertical_split, bounds["lower"]),
            (vertical_split, horizontal_split),
            (bounds["left"], horizontal_split)
        ],
        [
            (vertical_split, bounds["lower"]),
            (bounds["right"], bounds["lower"]),
            (bounds["right"], horizontal_split),
            (vertical_split, horizontal_split)
        ],
        [
            (bounds["left"], horizontal_split),
            (vertical_split, horizontal_split),
            (vertical_split, bounds["upper"]),
            (bounds["left"], bounds["upper"])
        ],
        [
            (vertical_split, horizontal_split),
            (bounds["right"], horizontal_split),
            (bounds["right"], bounds["upper"]),
            (vertical_split, bounds["upper"])
        ]
    ]
    possible_openings = [
        (
            random_odd_number_between_evens(bounds["left"], vertical_split),
            horizontal_split
        ),
        (
            random_odd_number_between_evens(vertical_split, bounds["right"]),
            horizontal_split
        ),
        (
            vertical_split,
            random_odd_number_between_evens(bounds["lower"], horizontal_split)
        ),
        (
            vertical_split,
            random_odd_number_between_evens(horizontal_split, bounds["upper"])
        ),
    ]
    deselected_index = random.randint(0, 3)
    openings = [
        opening for i, opening in enumerate(possible_openings)
        if i != deselected_index]
    return new_sections, openings


def divide_sections(sections: t.Sequence[Section]) -> t.Tuple[
        t.Sequence[Section],
        t.Sequence[Point]]:
    """
    Options:
    - return sections and openings
    - return list of walls to create

    Rules:
    - sections can only start/end on even numbers
    - openings can only be on odd numbers
    """
    divided: t.MutableSequence[Section] = []
    openings: t.MutableSequence[Point] = []
    for section in sections:
        if can_subdivide_section(section):
            new_sections, new_openings = divide_section(section)
            openings.extend(new_openings)
        else:
            new_sections = [section]
        divided.extend(new_sections)
    return divided, openings


def divide_section_recursive(section: Section) -> t.Tuple[
        t.Sequence[Section],
        t.Sequence[Point]]:
    divided = [section]
    openings = []
    while any(can_subdivide_section(section) for section in divided):
        divided, new_openings = divide_sections(divided)
        openings.extend(new_openings)
    return divided, openings


def write_section_to_grid(
        grid: t.Sequence[t.Sequence[str]],
        section: Section) -> None:
    directions = [
        (1, 0),
        (0, 1),
        (-1, 0),
        (0, -1)
    ]
    currentPos = section[0]
    bounds = get_bounds(section)
    for direction in directions:
        if direction[0] != 0:
            side_length = bounds["right"] - bounds["left"]
        else:
            side_length = bounds["upper"] - bounds["lower"]
        for i in range(side_length):
            currentPos = [
                currentPos[0] + direction[0],
                currentPos[1] + direction[1]]
            grid[currentPos[1]][currentPos[0]] = "XX"


def print_maze(
        enclosing_section: Section,
        sections: t.Sequence[Section],
        openings: t.Sequence[Point]) -> None:
    enclosing_bounds = get_bounds(enclosing_section)
    grid = [
        [
            "  "
            for i in range(
                enclosing_bounds["right"] - enclosing_bounds["left"] + 1)]
        for j in range(enclosing_bounds["upper"] - enclosing_bounds["lower"] + 1)
    ]
    write_section_to_grid(grid, enclosing_section)
    for section in sections:
        write_section_to_grid(grid, section)
    for opening in openings:
        grid[opening[1]][opening[0]] = "  "
    for row in grid:
        print("".join(row))


if __name__ == "__main__":
    size = int(sys.argv[1])
    enclosing_section = ((0, 0), (size, 0), (size, size), (0, size))
    divided_sections, openings = divide_section_recursive(enclosing_section)
    start = (1, 0)
    end = (size - 1, size)
    all_openings = [start, end, *openings]
    print_maze(enclosing_section, divided_sections, all_openings)
