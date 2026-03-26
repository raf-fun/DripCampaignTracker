# Campaign Reach Tracker and Initiator 

## The Problem

Marketers running SMS campaigns have no real-time visibility into whether they are on track to hit their engagement goals. After sending messages, they operate in the dark — they cannot tell who has responded positively, who has not replied yet, and whether intervention is needed.

My system provides a live campaign progress tracker that ingests incoming message replies, uses AI to classify each reply as a yes, no, or ambiguous, automatically sends appropriate responses, and sends progress to marketers.

## The User

Marketer 
My primary user. 
They can 
* create and launch campaigns
* set goals and follow-up rules
* monitor live progress
* manually trigger follow-up messages for leads who have not responded

---

## Architecture Overview

```
DripCampaignTracker/
├── DripCampaignTracker/          # ASP.NET Core 10 Web API
│   ├── Controllers/              # CampaignController, ConversationController, LeadController
│   ├── Data/                     # AppDbContext, DbSeeder
│   ├── DTOs/                     # Request and response shapes
│   ├── Entity/                   # Domain entities
│   ├── Enums/                    # EmployeeRole, CampaignStatus, ConversationStatus, SenderType, Classification
│   ├── Mappings/                 # AutoMapper profiles
│   └── Services/                 # AIService, MessageService, NotificationService
├── DripsCampaignTracker.Tests/   # xUnit test project
└── DripsCampaignTracker-UI/      # React + Vite frontend
```

## Concrete Requirements

**Campaign Management**
- Marketer creates a campaign with a target goal (min 10 leads), cooldown period (min 2 days), max 2 follow-ups per lead, and optional auto-close when goal is reached
- Leads are selected from existing records in the system at campaign creation — not created during launch
- Each campaign is owned by exactly one marketer

**Reply Ingestion & Classification**
- System exposes a POST endpoint to receive inbound SMS reply events
- Each reply is classified by AI (Google Gemini via Semantic Kernel) into Yes, No, or Ambiguous
- Classification determines the automated response sent back to the lead immediately

**Automated Responses**
- Yes: enthusiastic confirmation sent immediately
- No: polite thank-you sent immediately; lead is permanently removed from further outreach
- Ambiguous: generic follow-up message sent; lead remains eligible for future contact

**Follow-Up Rules**
- Marketer can manually trigger follow-ups for leads with no response or ambiguous replies
- Maximum 2 follow-ups per lead enforced by the system
- Minimum 2-day cooldown between messages enforced by the system
- Opted-out leads cannot be contacted again within the campaign

**Progress Tracking**
- Marketer dashboard shows all campaigns with Yes count vs goal, status, and cooldown settings
- Conversation view shows full message thread per lead with status and follow-up count
- UI enforces and communicates blocked states (opted out, max follow-ups reached, cooldown active)

### Core Flow

1. Marketer creates a campaign and selects leads
2. Marketer sends an initial outreach message per lead
3. Lead replies via SMS — reply is ingested via `POST /api/conversations/{id}/messages`
4. **AI (Google Gemini via Semantic Kernel)** classifies the reply as Yes, No, or Ambiguous
5. An automated response is generated and sent back to the lead immediately
6. Campaign progress is updated — Yes replies count toward the goal
7. Marketer monitors the live dashboard and sends follow-ups where eligible

---

## Key Assumptions

- Leads already exist in the system before a campaign is created
- A lead can be part of multiple campaigns simultaneously
- One marketer per campaign
- All messaging is SMS-based
- Ambiguous replies are treated as no response for goal-tracking purposes — they do not count toward the Yes goal
- The system enforces a minimum 2-day cooldown between messages and a maximum of 2 follow-ups per lead
- Leads who reply No are permanently opted out from that campaign

---

## Key Design Decisions & Tradeoffs

**In-Memory Database** 
EF Core in-memory provider with seeded data. Resets on restart giving a clean, predictable demo state every time. In production this would be a persistent relational database (SQL Server or Azure SQL).

**Semantic Kernel for AI** 
used Microsoft Semantic Kernel rather than raw HTTP calls to the AI provider. This keeps the AI provider swappable behind a common interface. The configuration is fully generic (BaseUrl, ApiKey, Model) so switching from Gemini to Azure OpenAI is a small change.

**AI Fallback** 
if the AI service is unavailable or errors, the system falls back to predefined response templates rather than failing. Classification defaults to Ambiguous on failure, keeping the lead in play rather than incorrectly opting them out.

**AutoMapper** 
used for entity-to-DTO mapping to keep controllers thin and mapping logic centralized.

**Interface over Concrete** 
AIService is registered behind IAIService, making it mockable in tests without needing to hit a real API.

**Notification Deduplication** 
milestone notifications are tracked in a HashSet on the NotificationService singleton to prevent duplicate SMS sends. In production this would be tracked in the database to survive restarts.


---

## AI Usage

**Tool:** Google Gemini (`gemini-3-flash-preview`) via Microsoft Semantic Kernel

**Why Semantic Kernel:** It abstracts the AI provider behind a common interface, making it straightforward to swap to Azure OpenAI in production with a small change.

**Where AI is used:**
- Classifying inbound lead replies as Yes, No, or Ambiguous
- Generating contextually appropriate automated responses (enthusiastic for Yes, gracious for No, low-pressure for Ambiguous)

**Risks and tradeoffs:**
- AI classification can be inconsistent on ambiguous or short replies — mitigated by defaulting to Ambiguous on low-confidence results
- API latency adds response time to the message flow — mitigated with loading state in the UI and fallback messages if the API fails
- Cost — each inbound reply triggers two AI calls (classify + generate). At scale this would need batching or caching strategies or better call management.

**AI-assisted development:** Claude chat and GitHub Copilot was used for boilerplate generation and code suggestions throughout the project.

---

## Running the Project

### Prerequisites
- .NET 10 SDK
- Node.js 22+

### Backend

```bash
cd DripCampaignTracker
dotnet user-secrets set "AI:ApiKey" "your-gemini-api-key"
dotnet user-secrets set "AI:BaseUrl" "https://generativelanguage.googleapis.com"
dotnet user-secrets set "AI:Model" "gemini-3-flash-preview"
dotnet run
```

API runs on `http://localhost:5066`
Swagger UI available at `http://localhost:5066/swagger`

### Frontend

```bash
cd DripsCampaignTracker-UI
npm install
npm run dev
```

Frontend runs on `http://localhost:5173`

### Tests

```bash
cd DripsCampaignTracker.Tests
dotnet test
```

---

## Seed Data

The application seeds two campaigns on startup for demo purposes.

**Campaign 1 — Spring Outreach** covers all conversation scenarios:
- Clark Yes-FirstContact — already confirmed
- Hal No-FirstContact — opted out
- Dinah Ambiguous-EligibleFollowUp — eligible for follow up
- Barry Ambiguous-MaxFollowUps — max follow ups reached
- Kara NoResponse-Eligible — eligible for follow up
- Oliver NoResponse-MaxFollowUps — max follow ups reached
- Zatanna NoResponse-CooldownActive — cooldown not yet passed
- Victor, Arthur, John — never contacted

**Campaign 2 — Summer Push** is seeded at 25% of goal (2/8 Yes). Use Wally, Barbara, Jason, Donna, Roy, and Mera to trigger 50%, 75%, and 100% milestones live during the demo.

---

## What I Would Improve With More Time

- **SignalR** — add real-time push so conversation threads update live without the marketer needing to refresh. The backend service layer is already structured to support this — it would be a hub broadcast after each message is processed
- **Authentication and authorization** — role-based access so marketers and managers see appropriate views. Currently simulated via a UI dropdown
- **Persistent database** — replace in-memory with SQL Server or Azure SQL with EF Core migrations
- **Background job for follow-ups** — automatically send follow-ups at the cooldown interval rather than relying on manual marketer action
- **Manager Messages** — Finish sending milestones. Is already set up but not finished.
- **Notification persistence** — track sent milestone notifications in the database instead of an in-memory HashSet so they survive restarts
- **Richer AI classification** — add confidence scoring and a human review queue for low-confidence replies
- **Lead management** — UI for creating and managing leads rather than relying on seeded data
