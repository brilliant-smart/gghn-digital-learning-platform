export type Difficulty = "Basic" | "Intermediate" | "Advanced";
export type Audience = "Policy Makers" | "Clinicians" | "Researchers" | "Community Health Workers";

export interface Resource {
  id: string;
  title: string;
  summary: string;
  topic: string;
  audience: Audience;
  difficulty: Difficulty;
  source: string;
  takeaways: string[];
}

export interface Course {
  id: string;
  title: string;
  description: string;
  topic: string;
  progress: number;
  lessons: { id: string; title: string; duration: string; completed: boolean }[];
}

export interface Pathway {
  id: string;
  title: string;
  description: string;
  resourceCount: number;
  topic: string;
}

export interface Template {
  id: string;
  title: string;
  description: string;
  format: string;
}

export interface Speaker {
  id: string;
  name: string;
  role: string;
  org: string;
}

export const TOPICS = [
  "Health Systems Strengthening",
  "Infectious Disease Control",
  "HIV/AIDS Programs",
  "Maternal & Child Health",
  "Disease Surveillance",
  "Community Health",
  "Public Health Policy",
];

export const resources: Resource[] = [
  {
    id: "r1",
    title: "Strengthening HIV Service Delivery in Sub-Saharan Africa",
    summary:
      "An evidence-based overview of differentiated service delivery models that improve HIV treatment outcomes while reducing health system burden.",
    topic: "HIV/AIDS Programs",
    audience: "Clinicians",
    difficulty: "Intermediate",
    source: "https://example.org/hiv-dsd",
    takeaways: [
      "Differentiated service delivery improves retention in care",
      "Community ART refill models reduce facility congestion",
      "Viral load monitoring remains central to treatment success",
    ],
  },
  {
    id: "r2",
    title: "Disease Surveillance Frameworks for Outbreak Preparedness",
    summary:
      "A practical guide to building integrated disease surveillance systems, drawing on lessons from recent epidemics across Africa.",
    topic: "Disease Surveillance",
    audience: "Policy Makers",
    difficulty: "Advanced",
    source: "https://example.org/surveillance",
    takeaways: [
      "Early warning systems reduce outbreak response times",
      "Data integration across facilities is essential",
      "Community-based surveillance strengthens detection",
    ],
  },
  {
    id: "r3",
    title: "Maternal Health Indicators: A Policy Brief",
    summary:
      "Plain-language summary of key maternal mortality indicators and their implications for national health policy in Nigeria.",
    topic: "Maternal & Child Health",
    audience: "Policy Makers",
    difficulty: "Basic",
    source: "https://example.org/maternal-brief",
    takeaways: [
      "Skilled birth attendance correlates strongly with reduced mortality",
      "Antenatal care coverage gaps remain in rural areas",
      "Investment in midwifery yields high returns",
    ],
  },
  {
    id: "r4",
    title: "Building Resilient Primary Healthcare Systems",
    summary:
      "Frameworks for strengthening primary healthcare delivery as the foundation of universal health coverage.",
    topic: "Health Systems Strengthening",
    audience: "Researchers",
    difficulty: "Intermediate",
    source: "https://example.org/phc",
    takeaways: [
      "Primary care is the most cost-effective entry point",
      "Health workforce planning is critical",
      "Financing reform must accompany service reform",
    ],
  },
  {
    id: "r5",
    title: "Community Health Worker Programs at Scale",
    summary:
      "Lessons learned from scaling community health worker programs across multiple African contexts.",
    topic: "Community Health",
    audience: "Community Health Workers",
    difficulty: "Basic",
    source: "https://example.org/chw",
    takeaways: [
      "Standardized training improves quality of care",
      "Supportive supervision drives retention",
      "Digital tools enhance CHW productivity",
    ],
  },
  {
    id: "r6",
    title: "Tuberculosis Control: Updated WHO Guidelines",
    summary:
      "Synthesis of the latest WHO tuberculosis treatment guidelines with implementation considerations for low-resource settings.",
    topic: "Infectious Disease Control",
    audience: "Clinicians",
    difficulty: "Advanced",
    source: "https://example.org/tb-guidelines",
    takeaways: [
      "Shorter regimens improve adherence",
      "Drug-resistant TB requires specialized programs",
      "Active case finding remains a priority",
    ],
  },
];

export const courses: Course[] = [
  {
    id: "c1",
    title: "Foundations of Health Systems Strengthening",
    description:
      "An introductory course covering the WHO health system building blocks and their application in African contexts.",
    topic: "Health Systems Strengthening",
    progress: 60,
    lessons: [
      { id: "l1", title: "Introduction to health systems", duration: "12 min", completed: true },
      { id: "l2", title: "Service delivery models", duration: "18 min", completed: true },
      { id: "l3", title: "Health workforce planning", duration: "22 min", completed: true },
      { id: "l4", title: "Health financing", duration: "20 min", completed: false },
      { id: "l5", title: "Information systems & governance", duration: "15 min", completed: false },
    ],
  },
  {
    id: "c2",
    title: "HIV Treatment & Care Essentials",
    description:
      "A clinical training course on current HIV treatment protocols, viral load monitoring, and patient-centered care.",
    topic: "HIV/AIDS Programs",
    progress: 25,
    lessons: [
      { id: "l1", title: "HIV epidemiology overview", duration: "10 min", completed: true },
      { id: "l2", title: "ART initiation", duration: "20 min", completed: false },
      { id: "l3", title: "Viral load monitoring", duration: "15 min", completed: false },
      { id: "l4", title: "Managing co-infections", duration: "18 min", completed: false },
    ],
  },
  {
    id: "c3",
    title: "Outbreak Investigation Fundamentals",
    description:
      "Step-by-step methods for investigating disease outbreaks, with case studies from recent African epidemics.",
    topic: "Disease Surveillance",
    progress: 0,
    lessons: [
      { id: "l1", title: "Defining an outbreak", duration: "12 min", completed: false },
      { id: "l2", title: "Case definitions & line lists", duration: "20 min", completed: false },
      { id: "l3", title: "Descriptive epidemiology", duration: "25 min", completed: false },
      { id: "l4", title: "Control measures", duration: "18 min", completed: false },
    ],
  },
];

export const pathways: Pathway[] = [
  {
    id: "p1",
    title: "Health Systems Strengthening",
    description:
      "A curated learning journey through the building blocks of resilient health systems, from financing to workforce.",
    resourceCount: 12,
    topic: "Health Systems Strengthening",
  },
  {
    id: "p2",
    title: "Infectious Disease Control",
    description:
      "Build expertise in preventing, detecting, and responding to infectious disease threats in African contexts.",
    resourceCount: 9,
    topic: "Infectious Disease Control",
  },
  {
    id: "p3",
    title: "Community Health Programs",
    description:
      "Learn to design, implement, and scale community-based health interventions with measurable impact.",
    resourceCount: 8,
    topic: "Community Health",
  },
  {
    id: "p4",
    title: "Maternal & Child Health",
    description:
      "Evidence-based approaches to reducing maternal and child mortality across the continuum of care.",
    resourceCount: 10,
    topic: "Maternal & Child Health",
  },
];

export const templates: Template[] = [
  {
    id: "t1",
    title: "Monitoring & Evaluation Framework Template",
    description: "A ready-to-adapt M&E framework for public health programs, with indicator examples.",
    format: "DOCX",
  },
  {
    id: "t2",
    title: "Training Facilitator Guide",
    description: "Structured facilitator guide for delivering 1- to 3-day public health training workshops.",
    format: "PDF",
  },
  {
    id: "t3",
    title: "Community Health Worker Supervision Checklist",
    description: "Field-tested supervision checklist for routine CHW support and quality assurance.",
    format: "PDF",
  },
  {
    id: "t4",
    title: "Outbreak Investigation Line List",
    description: "Standard line list template for capturing case data during outbreak investigations.",
    format: "XLSX",
  },
];

export type PublicationType = "Journal Article" | "Report" | "Policy Brief" | "Conference Abstract";

export interface Publication {
  id: string;
  title: string;
  authors: string[];
  type: PublicationType;
  year: number;
  summary: string;
  tags: string[];
  keyFindings: string[];
  externalUrl: string;
}

export const PUBLICATION_TYPES: PublicationType[] = [
  "Journal Article",
  "Report",
  "Policy Brief",
  "Conference Abstract",
];

export const PUBLICATION_TOPICS = [
  "HIV/AIDS",
  "Infectious Disease",
  "Health Systems",
  "Digital Health",
  "Maternal Health",
  "Community Health",
  "Service Delivery",
];

export const publications: Publication[] = [
  {
    id: "pub1",
    title: "Patients and Healthcare Workers' Preferences for Smart Locker-Based Medication Access in Nigeria",
    authors: ["Ibrahim Bola Gobir", "Piring'ar Mercy Niyang", "Samson Agboola"],
    type: "Journal Article",
    year: 2024,
    summary:
      "Explores the acceptability and usability of smart locker systems for dispensing chronic disease medication in Nigeria, highlighting patient convenience and system efficiency.",
    tags: ["Digital Health", "Health Systems"],
    keyFindings: [
      "Patients reported high satisfaction with 24/7 medication pickup convenience",
      "Healthcare workers noted reduced facility congestion and shorter queues",
      "Smart lockers improved adherence among patients with chronic conditions",
    ],
    externalUrl: "https://example.org/publications/smart-locker-preferences",
  },
  {
    id: "pub2",
    title: "Acceptability of Smart Locker Technology for Chronic Disease Medication Delivery",
    authors: ["Ibrahim Bola Gobir", "Mercy Niyang", "Havilah Nnadozie"],
    type: "Journal Article",
    year: 2024,
    summary:
      "Evaluates patient and provider perspectives on technology-enabled medication access solutions in resource-constrained settings.",
    tags: ["Digital Health", "Service Delivery"],
    keyFindings: [
      "Over 80% of surveyed patients expressed willingness to use the technology",
      "Providers cited workflow integration as a key implementation factor",
      "Cost and infrastructure remain primary scale-up considerations",
    ],
    externalUrl: "https://example.org/publications/smart-locker-acceptability",
  },
  {
    id: "pub3",
    title: "Strengthening Lassa Fever Prevention Through Community Engagement in Ondo State",
    authors: ["Ibrahim Bola Gobir", "Mercy Niyang"],
    type: "Conference Abstract",
    year: 2025,
    summary:
      "Demonstrates how community-led interventions improved awareness and behavioral practices in Lassa fever prevention.",
    tags: ["Infectious Disease", "Community Health"],
    keyFindings: [
      "Community health volunteers reached over 50,000 residents with prevention messaging",
      "Household sanitation practices improved measurably post-intervention",
      "Local leadership engagement was central to behavior change",
    ],
    externalUrl: "https://example.org/publications/lassa-ondo",
  },
  {
    id: "pub4",
    title: "Bridging the Gap Between Awareness and Behavior in Lassa Fever Prevention",
    authors: ["GGHN Research Team"],
    type: "Conference Abstract",
    year: 2025,
    summary:
      "Examines behavioral gaps in disease prevention despite high awareness levels in endemic communities.",
    tags: ["Infectious Disease"],
    keyFindings: [
      "Awareness of Lassa fever exceeded 85% but protective behaviors lagged",
      "Structural barriers limited adoption of recommended practices",
      "Tailored behavior-change communication is needed alongside awareness campaigns",
    ],
    externalUrl: "https://example.org/publications/lassa-awareness-behavior",
  },
  {
    id: "pub5",
    title: "Improving Retention in HIV Care Through Ancillary Support Services",
    authors: ["Mercy Niyang", "Ibrahim Bola Gobir"],
    type: "Journal Article",
    year: 2024,
    summary:
      "Analyzes the role of support services in improving retention and outcomes for HIV patients in Nigeria.",
    tags: ["HIV/AIDS", "Health Systems"],
    keyFindings: [
      "Patients receiving ancillary support showed higher 12-month retention",
      "Transport and nutrition support were the most-cited enablers",
      "Integrated service models outperformed siloed delivery",
    ],
    externalUrl: "https://example.org/publications/hiv-retention",
  },
  {
    id: "pub6",
    title: "Community-Based PMTCT Models in Northern Nigeria: Operational Challenges and Lessons",
    authors: ["GGHN Research Team"],
    type: "Journal Article",
    year: 2024,
    summary:
      "Compares different approaches to prevention of mother-to-child transmission of HIV in underserved communities.",
    tags: ["Maternal Health", "HIV/AIDS"],
    keyFindings: [
      "Community-based models reached women who did not access facility-based care",
      "Workforce training and supervision were the largest operational gaps",
      "Mother-mentor approaches strengthened retention through pregnancy and postpartum",
    ],
    externalUrl: "https://example.org/publications/pmtct-northern-nigeria",
  },
  {
    id: "pub7",
    title: "Digital Health Innovations for Medication Access in Low-Resource Settings",
    authors: ["Sonia Ogbeh", "GGHN Digital Health Team"],
    type: "Report",
    year: 2023,
    summary:
      "Highlights innovative approaches to improving medication access through digital health interventions.",
    tags: ["Digital Health"],
    keyFindings: [
      "Digital tools can reduce stock-out durations when integrated with supply chain data",
      "User-centered design is critical to adoption in low-resource settings",
      "Sustainable financing models remain the key barrier to scale",
    ],
    externalUrl: "https://example.org/publications/digital-health-access",
  },
  {
    id: "pub8",
    title: "Annual Report: Advancing Health Systems Strengthening in Nigeria",
    authors: ["Georgetown Global Health Nigeria"],
    type: "Report",
    year: 2023,
    summary:
      "Overview of programs, research impact, and health system interventions implemented across Nigeria.",
    tags: ["Health Systems"],
    keyFindings: [
      "Programs reached over 1 million beneficiaries across multiple states",
      "Research outputs informed national HIV and disease surveillance policy",
      "Partnerships with government and academia expanded implementation reach",
    ],
    externalUrl: "https://example.org/publications/annual-report-2023",
  },
];

export const speakers: Speaker[] = [
  { id: "s1", name: "Dr. Ibrahim Bola Gobir", role: "Chief Executive Officer", org: "GGHN" },
  { id: "s2", name: "Ms. Piring'ar Mercy Niyang", role: "Chief Technical Officer", org: "GGHN" },
  { id: "s3", name: "Emeka Madubuko", role: "Director, Health Informatics", org: "GGHN" },
  { id: "s4", name: "Ms. Ochanya Sonia Ogbeh", role: "Advisor, Digital Health Innovations & Gender", org: "GGHN" },
  { id: "s5", name: "Adebola Akinjeji", role: "Health Informatics Advisor", org: "GGHN" },
  { id: "s6", name: "Dr. Winifred Ukponu", role: "Associate Director, Global Health Security", org: "GGHN" },
];
