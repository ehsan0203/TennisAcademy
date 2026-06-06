---
name: project-context
description: Domain knowledge and product specification for the MTA Academy platform. Use this context to understand what the system does, its modules, entities, and end-to-end user flow before implementing any feature.
---

# Project Context — MTA Academy

You are working on an **online sports club & coaching platform** (racket sport, e.g. tennis).
This document describes the product domain, modules, and core flows. Use it to understand
*what* the system does. Coding rules and conventions live separately in `CLAUDE.md`.

## Roles

The system has three roles:

- **User (Athlete)** — takes the level test, receives a program, buys a subscription, sends tickets.
- **Coach** — sees their students, edits workout programs, creates nutrition plans, answers tickets.
- **Admin** — full control of the system.

## High-Level Goal

After registering, a user takes an initial assessment test, receives a training program,
optionally chooses a coach, buys a subscription, and then manages their workout program,
nutrition plan, and coach communication online.

## Core Backbone

Technically, the heart of the project revolves around four pillars, and every other module
is built around them:

1. **Rule Engine**
2. **WorkoutProgram**
3. **Ticket System**
4. **Subscription System**

---

## Modules

### 1. Registration & Login
- Sign up with email + password, or sign in with Google.
- On registration, a `User` and a `UserProfile` are created together.
- Profile fields: first name, last name, year of birth, playing level, years of experience, profile photo.

### 2. Coach Registration
- Any user can request to become a coach; the system creates a `CoachProfile`.
- Documents uploaded: coaching card, federation certificate, international certificate.
- Approval status is tracked via `IsApproved`.
- When the admin approves, the Coach Dashboard becomes active.

### 3. Level Assessment Test
- The user starts the test; questions are pulled from `Questions`.
- Example question: "How many years of experience do you have? (0–1 / 1–3 / 3–5 / more)".
- Answers are stored in `UserAnswers`.

### 4. Rule Engine (decision engine)
- After the test ends, the system evaluates all active templates.
- Each template has conditions stored in `RuleCondition` (e.g. age > 18, level = beginner, goal = weight loss).
- Algorithm: active templates → check conditions → full match → priority → select best template.

### 5. Program Materialization
- A template is only a blueprint. After a template is selected, a real program is built for the user.
- A `WorkoutProgram` is created and data is copied:
  - `TemplateWorkoutDay` → `WorkoutDay`
  - `TemplateWorkoutExercise` → `WorkoutExercise`
- This copy process is called **Materialization**.

### 6. Workout Program Structure
- `WorkoutProgram` → contains `WorkoutDay` → each day contains `WorkoutExercise`.
- Example: Program → Day1 (Exercise1, Exercise2), Day2 (Exercise1), Day3 (…).

### 7. Program Versioning
- Each program has a `Version`. Version 1 is the initial program; each coach edit creates a new
  version (Version 2, 3, …) to keep a change history.

### 8. Choosing a Coach
After receiving a program, the user has two options:
- **Without a coach** — just use the generated program.
- **With a coach** — choose from a coach list showing: name, photo, résumé, certificates, rating, number of students.

### 9. Subscriptions
- To communicate with a coach, the user must buy a subscription.
- Plan durations include: Monthly, Quarterly, VIP — stored in `SubscriptionPlan`.
- **Discipline scope (NEW):** a subscription can be purchased for:
  - **Sport only** (the racket sport, e.g. tennis), or
  - **Bodybuilding only**, or
  - **Both** (sport + bodybuilding).
  The selected discipline scope determines which programs, coaches, and content the
  subscription unlocks.
- Purchase flow: `Payment` → on success a `UserSubscription` is created.

### 10. Ticket System (new model — replaces chat)
- Chat has been replaced by tickets.
- Each user has **1 free credit**. Sending a ticket consumes **1 credit**.
- Before creating a ticket, the user picks a topic (e.g. Forehand, Backhand, Serve, Footwork).
- `Ticket` fields: title, topic, status, date, user, coach.
- Each ticket contains `TicketMessage` items. Message types: text, image, video, audio, GIF.
- Structure: Ticket → Message1, Message2, Message3 …

### 11. Video Analysis by Coach
- The user sends a training video; the coach reviews it and replies
  (e.g. "racket angle is wrong", "keep your left foot forward").
- This is the most important capability of the project.

### 12. Nutrition Plan
- A coach can create a `NutritionPlan`.
- Structure: `NutritionPlan` → `NutritionDay` → Breakfast / Lunch / Dinner.
- Each meal has `MealFood` items (e.g. breakfast: 2 eggs, 1 toast, 1 glass of milk).

### 13. Calorie Calculation
- Each food has Calories, Protein, Carb, Fat.
- The system sums them into `DailyCalories`.

### 14. GIF Library
- Used to teach movements. Relationship: `Exercise` → `GIF` (e.g. Forehand, Backhand, Serve).
- A coach uploads a GIF (status `Pending`); the admin approves it; then all coaches can use it.

### 15. Coach Panel
- Dashboard includes: students, programs, nutrition plans, tickets, profile, GIF library.

### 16. Admin Panel
- The largest part of the project. Manages: Users, Coaches, Questions, Exercises, GIFs,
  Templates, Plans, Payments, Tickets.

### 17. Public Site Home Page
- Sections: Courses (sells training courses), Plans (training plans), About Us, FAQ,
  and an intro video for each course and plan.

### 18. User Progress Tracking
- The user can record completion of exercises and meals.
- Tables: `WorkoutExerciseProgress`, `MealProgress` (e.g. "Squat ✓ done", "Breakfast ✓ eaten").

### 19. Cron Jobs (scheduled tasks)
- **Subscription expired** → lock ticket sending.
- **Workout reminder** → "Today is your workout day."
- **Coach reminder** → "You have 3 unanswered tickets."

---

## End-to-End Flow

Register → complete profile → level test → save answers → Rule Engine → select template →
build WorkoutProgram → view initial program → choose coach → buy subscription →
access tickets → send training video → coach review → edit program → receive nutrition plan →
record daily progress → renew subscription.
