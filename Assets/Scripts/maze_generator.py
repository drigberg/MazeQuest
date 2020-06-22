import random
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
        "left": min(point[0] for point in section),
        "right": max(point[0] for point in section),
        "lower": min(point[1] for point in section),
        "upper": max(point[1] for point in section),
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
    left_bound = min(point[0] for point in section)
    right_bound = max(point[0] for point in section)
    lower_bound = min(point[1] for point in section)
    upper_bound = max(point[1] for point in section)
    vertical_split = random_even_number_between_evens(
        left_bound, right_bound)
    horizontal_split = random_even_number_between_evens(
        lower_bound, upper_bound)
    new_sections = [
        [
            (left_bound, lower_bound),
            (vertical_split, lower_bound),
            (vertical_split, horizontal_split),
            (left_bound, horizontal_split)
        ],
        [
            (vertical_split, lower_bound),
            (right_bound, lower_bound),
            (right_bound, horizontal_split),
            (vertical_split, horizontal_split)
        ],
        [
            (left_bound, horizontal_split),
            (vertical_split, horizontal_split),
            (vertical_split, upper_bound),
            (left_bound, upper_bound)
        ],
        [
            (vertical_split, horizontal_split),
            (right_bound, horizontal_split),
            (right_bound, upper_bound),
            (vertical_split, upper_bound)
        ]
    ]
    possible_openings = [
        (
            random_odd_number_between_evens(left_bound, vertical_split),
            horizontal_split
        ),
        (
            random_odd_number_between_evens(vertical_split, right_bound),
            horizontal_split
        ),
        (
            vertical_split,
            random_odd_number_between_evens(lower_bound, horizontal_split)
        ),
        (
            vertical_split,
            random_odd_number_between_evens(horizontal_split, upper_bound)
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


def divide_sections_recursive(sections: t.Sequence[Section]) -> t.Tuple[
        t.Sequence[Section],
        t.Sequence[Point]]:
    divided = [section for section in sections]
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
    size = 40
    enclosing_section = ((0, 0), (size, 0), (size, size), (0, size))
    sections = [enclosing_section]
    divided_sections, openings = divide_sections_recursive(sections)
    start = (1, 0)
    end = (size - 1, size)
    all_openings = [start, end, *openings]
    print_maze(enclosing_section, divided_sections, all_openings)
