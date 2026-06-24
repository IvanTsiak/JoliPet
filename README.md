# JoliPet API
JoliPet is a backend API for a virtual pet simulation game with RPG and PvP mechanics. The application operates strictly as an API service without a built-in frontend client. It handles user authentication, pet lifecycle management, dynamic state decay, and interactive mechanics such as battles and text-based communication.

## Features
- **Cookie-based Authentication**: Secure user registration and login using BCrypt password hashing and HTTP-only cookies.
- **Dynamic State Decay**: Pet mood mathematically decreases over time based on the specific pet type's equilibrium and decay constants. State is calculated dynamically upon data retrieval.
- **Interactive Chat System**: Text messages sent to the pet are parsed and analyzed. Specific words carry weight that directly influences the pet's mood and grants experience points.
- **PvP Arena**: An asynchronous battle system that calculates combat power based on pet stats, applies RNG modifiers, and updates states (damage, healing, XP) for both attacker and defender.
- **Notification System**: Internal logging for crucial events (level-ups, battle results, pet deaths) with read/unread tracking.

## Technology Stack
- C#
- ASP.NET Core Web API
- Entity Framework Core
- BCrypt.Net

## API Reference
### Authentication (`/api/Auth`)

| Method | Endpoint | Description |
| :--- | :--- | :--- |
| `POST` | `/api/Auth/register` | Registers a new user. Expects `Username`, `Email`, and `Password`. |
| `POST` | `/api/Auth/login` | Authenticates a user and sets an HTTP-only authorization cookie. |
| `POST` | `/api/Auth/logout` | Clears the current authentication cookie. |
| `GET`  | `/api/Auth/whoami` | Returns the ID, Name, and Email of the currently authenticated user. |
| `GET`  | `/api/Auth/guest-info` | Returns public server statistics (total users, alive/dead pets, available pet types). |

### Pets Management (`/api/Pets`)

| Method | Endpoint | Description |
| :--- | :--- | :--- |
| `GET`  | `/api/Pets/types` | Retrieves a list of all available pet types for creation. |
| `GET`  | `/api/Pets/my` | Retrieves the current user's alive pet. Automatically calculates and applies time-based mood decay before returning the data. |
| `POST` | `/api/Pets` | Creates a new pet for the user (requires `TypeId` and `Name`). Fails if the user already has an active pet. |
| `POST` | `/api/Pets/abandon` | Changes the current pet's status to dead (abandons the pet). |

### Battles (`/api/Battles`)

| Method | Endpoint | Description |
| :--- | :--- | :--- |
| `GET`  | `/api/Battles/targets` | Returns a list of alive pets belonging to other users that fall within the allowed level difference for a fair battle. |
| `POST` | `/api/Battles/{id}/attack` | Initiates a battle against the target pet ID. Returns the battle outcome (damage, victory/loss status) and generates a notification. |

### Interactions (`/api/Interactions`)

| Method | Endpoint | Description |
| :--- | :--- | :--- |
| `POST` | `/api/Interactions/talk` | Accepts a text string. Analyzes the words against the database to calculate word weight, updates the pet's mood/XP, and returns the net change. |

### Notifications (`/api/Notifications`)

| Method | Endpoint | Description |
| :--- | :--- | :--- |
| `GET`  | `/api/Notifications` | Retrieves all unread notifications for the authenticated user. |
| `POST` | `/api/Notifications/{id}/read` | Marks a specific notification as read. |

### Cemetery (`/api/Cemetery`)

| Method | Endpoint | Description |
| :--- | :--- | :--- |
| `GET`  | `/api/Cemetery` | Returns a global history list of all pets that have died or been abandoned, including their final stats and time of death. |
