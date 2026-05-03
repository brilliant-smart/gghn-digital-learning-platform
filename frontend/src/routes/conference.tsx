import { createFileRoute } from "@tanstack/react-router";
import { useEffect, useState, useRef } from "react";
import { SiteShell } from "@/components/layout/SiteShell";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Accordion, AccordionContent, AccordionItem, AccordionTrigger } from "@/components/ui/accordion";
import { Calendar, MapPin, Clock, Video, FileText, Users, Globe, Mic, ExternalLink, ChevronDown, Sparkles } from "lucide-react";
import { VenueMap } from "@/components/VenueMap";
import { PartnerCarousel } from "@/components/PartnerCarousel";
import { conferenceApi, type ConferenceDto, type SpeakerDto, type SessionDto } from "@/api/conferences";
import { ConferenceRegistrationModal } from "@/components/ConferenceRegistrationModal";

export const Route = createFileRoute("/conference")({
  head: () => ({
    meta: [
      { title: "Annual Conference | GGHN Digital Learning" },
      { name: "description", content: "GGHN Annual Global Health Conference: themes, agenda, speakers, and registration." },
    ],
  }),
  component: ConferencePage,
});

const TRACK_COLORS: Record<string, string> = {
  Plenary: "bg-blue-100 text-blue-700",
  "Global Health Security": "bg-emerald-100 text-emerald-700",
  "Digital Health Innovation": "bg-purple-100 text-purple-700",
  "Health Systems Strengthening": "bg-amber-100 text-amber-700",
  Break: "bg-muted text-muted-foreground",
  Special: "bg-rose-100 text-rose-700",
};

const TRACK_ICONS: Record<string, string> = {
  Plenary: "bg-blue-500",
  "Global Health Security": "bg-emerald-500",
  "Digital Health Innovation": "bg-purple-500",
  "Health Systems Strengthening": "bg-amber-500",
  Special: "bg-rose-500",
};

function groupSessionsByDay(sessions: SessionDto[]) {
  const map = new Map<string, SessionDto[]>();
  for (const s of sessions) {
    const d = new Date(s.startTime).toLocaleDateString('en-US', { weekday: 'long', month: 'short', day: 'numeric' });
    if (!map.has(d)) map.set(d, []);
    map.get(d)!.push(s);
  }
  const entries = Array.from(map.entries()).map(([day, sessions]) => ({
    day,
    sessions: sessions.sort((a, b) => new Date(a.startTime).getTime() - new Date(b.startTime).getTime()),
  }));
  entries.sort((a, b) => new Date(a.sessions[0].startTime).getTime() - new Date(b.sessions[0].startTime).getTime());
  return entries;
}

function formatTime(iso: string) {
  return new Date(iso).toLocaleTimeString('en-US', { hour: 'numeric', minute: '2-digit' });
}

function formatDateRange(start: string, end: string) {
  const s = new Date(start);
  const e = new Date(end);
  const opts: Intl.DateTimeFormatOptions = { month: 'long', day: 'numeric' };
  if (s.getFullYear() !== e.getFullYear()) {
    return `${s.toLocaleDateString('en-US', { ...opts, year: 'numeric' })} – ${e.toLocaleDateString('en-US', { ...opts, year: 'numeric' })}`;
  }
  return `${s.toLocaleDateString('en-US', opts)} – ${e.toLocaleDateString('en-US', { ...opts, year: 'numeric' })}`;
}

function getInitials(name: string) {
  return name.replace(/^(Dr\.|Ms\.|Mr\.|Prof\.|Mrs\.)\s*/, "").split(" ").map((n) => n[0]).slice(0, 2).join("");
}

function ConferencePage() {
  const [conferences, setConferences] = useState<ConferenceDto[]>([]);
  const [speakers, setSpeakers] = useState<SpeakerDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [showRegistration, setShowRegistration] = useState(false);
  const [activeDay, setActiveDay] = useState(0);
  const [expandedSpeaker, setExpandedSpeaker] = useState<string | null>(null);
  const agendaRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    Promise.all([
      conferenceApi.getAll().catch(() => [] as ConferenceDto[]),
      conferenceApi.getSpeakers().catch(() => [] as SpeakerDto[]),
    ]).then(([c, s]) => {
      setConferences(c);
      setSpeakers(s);
    }).finally(() => setLoading(false));
  }, []);

  if (loading) {
    return (
      <SiteShell>
        <div className="flex min-h-[60vh] items-center justify-center">
          <p className="text-muted-foreground">Loading conference...</p>
        </div>
      </SiteShell>
    );
  }

  const conf = conferences[0];
  if (!conf) {
    return (
      <SiteShell>
        <div className="flex min-h-[60vh] items-center justify-center">
          <div className="text-center">
            <h2 className="text-xl font-bold">No Conference Found</h2>
            <p className="mt-2 text-muted-foreground">Check back soon for upcoming conference announcements.</p>
          </div>
        </div>
      </SiteShell>
    );
  }

  const sessionsByDay = groupSessionsByDay(conf.sessions || []);
  const isArchived = conf.isArchived;
  const sessionCount = conf.sessions?.length || 0;
  const speakerCount = speakers.length;
  const sponsorCount = conf.sponsors?.length || 0;

  return (
    <SiteShell>
      <section
        className="relative overflow-hidden"
        style={{ backgroundImage: "var(--gradient-hero)" }}
      >
        <div className="pointer-events-none absolute -right-24 -top-24 h-96 w-96 rounded-full bg-white/10 blur-3xl" />
        <div className="pointer-events-none absolute -bottom-32 -left-24 h-96 w-96 rounded-full bg-white/5 blur-3xl" />

        <div className="relative mx-auto max-w-7xl px-4 py-20 sm:px-6 lg:px-8 lg:py-28">
          <div className="max-w-3xl">
            <div className="flex items-center gap-2 mb-4">
              <Badge className="border-0 bg-white/15 text-white hover:bg-white/20">
                {conf.year} Edition
              </Badge>
              <Badge variant="outline" className="border-white/20 text-white/70">
                {isArchived ? "Archived" : "Upcoming"}
              </Badge>
            </div>

            <h1 className="text-4xl font-bold tracking-tight text-white sm:text-5xl lg:text-6xl">
              {conf.title}
            </h1>
            <p className="mt-4 text-xl text-white/80 italic sm:text-2xl">
              {conf.theme}
            </p>
            <p className="mt-5 max-w-2xl text-base leading-relaxed text-white/85 sm:text-lg">
              {conf.description}
            </p>

            <div className="mt-6 flex flex-wrap items-center gap-x-6 gap-y-2 text-white/90">
              <span className="inline-flex items-center gap-2 text-sm">
                <Calendar className="h-4 w-4" />
                {formatDateRange(conf.startDate, conf.endDate)}
              </span>
              <span className="inline-flex items-center gap-2 text-sm">
                <MapPin className="h-4 w-4" />
                {conf.venue}
              </span>
            </div>

            <div className="mt-8 flex flex-wrap gap-3">
              {!isArchived && (
                <Button size="lg" onClick={() => setShowRegistration(true)} className="bg-white text-primary hover:bg-white/90">
                  Register Now
                </Button>
              )}
              <Button
                size="lg"
                variant="outline"
                className="border-white/30 bg-white/10 text-white hover:bg-white/20"
                onClick={() => agendaRef.current?.scrollIntoView({ behavior: "smooth" })}
              >
                Explore Agenda
              </Button>
            </div>

            {/* Stats row */}
            <div className="mt-10 flex flex-wrap gap-6">
              {[
                { icon: Clock, label: "Days", value: "3" },
                { icon: Mic, label: "Sessions", value: String(sessionCount) },
                { icon: Users, label: "Speakers", value: String(speakerCount) },
                { icon: Globe, label: "Sponsors", value: String(sponsorCount) },
              ].map((s) => (
                <div key={s.label} className="flex items-center gap-3 text-white">
                  <div className="flex h-10 w-10 items-center justify-center rounded-lg bg-white/10">
                    <s.icon className="h-5 w-5" />
                  </div>
                  <div>
                    <p className="text-2xl font-bold">{s.value}</p>
                    <p className="text-xs text-white/60">{s.label}</p>
                  </div>
                </div>
              ))}
            </div>
          </div>
        </div>
      </section>

      <section className="bg-background py-16 sm:py-20">
        <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
          <div className="grid gap-12 lg:grid-cols-2 lg:items-center">
            <div>
              <Badge variant="secondary" className="mb-4">About the Conference</Badge>
              <h2 className="text-2xl font-bold tracking-tight sm:text-3xl">
                Where Evidence Meets Action
              </h2>
              <p className="mt-4 text-muted-foreground leading-relaxed">
                The GGHN Annual Global Health Conference is Nigeria's premier gathering of public health
                professionals, researchers, policymakers, and development partners. Over three days, we bring
                together the brightest minds to share evidence, forge partnerships, and chart the course for
                stronger health systems across Africa.
              </p>
            </div>
            <div className="grid gap-4 sm:grid-cols-3">
              {[
                { icon: Sparkles, title: "Learn", desc: "Cutting-edge research, practical tools, and implementation science from across the continent." },
                { icon: Users, title: "Connect", desc: "Network with 500+ peers, funders, and thought leaders in global health and development." },
                { icon: Globe, title: "Advance", desc: "Shape national and regional health policy through evidence-backed recommendations." },
              ].map((c) => (
                <Card key={c.title} className="text-center">
                  <CardContent className="pt-6">
                    <div className="mx-auto flex h-12 w-12 items-center justify-center rounded-full bg-primary-soft">
                      <c.icon className="h-6 w-6 text-primary" />
                    </div>
                    <h3 className="mt-4 font-semibold">{c.title}</h3>
                    <p className="mt-2 text-xs text-muted-foreground">{c.desc}</p>
                  </CardContent>
                </Card>
              ))}
            </div>
          </div>
        </div>
      </section>

      {speakers.length > 0 && (
        <section className="bg-muted/30 py-16 sm:py-20">
          <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
            <div className="text-center mb-10">
              <Badge variant="secondary" className="mb-3">Featured Speakers</Badge>
              <h2 className="text-2xl font-bold tracking-tight sm:text-3xl">Voices Shaping Global Health</h2>
              <p className="mt-2 text-muted-foreground">Meet the experts and practitioners leading our sessions</p>
            </div>
            <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
              {speakers.slice(0, 6).map((s) => (
                <Card
                  key={s.id}
                  className="cursor-pointer hover:shadow-md transition-shadow"
                  onClick={() => setExpandedSpeaker(expandedSpeaker === s.id ? null : s.id)}
                >
                  <CardHeader className="flex-row items-center gap-4 space-y-0">
                    <div className="flex h-14 w-14 flex-shrink-0 items-center justify-center rounded-full bg-primary-soft text-lg font-semibold text-primary">
                      {getInitials(s.name)}
                    </div>
                    <div className="min-w-0">
                      <CardTitle className="text-base">{s.name}</CardTitle>
                      <CardDescription>{s.title}</CardDescription>
                      <CardDescription className="text-xs">{s.organization}</CardDescription>
                    </div>
                  </CardHeader>
                  {expandedSpeaker === s.id && s.bio && (
                    <CardContent>
                      <p className="text-sm text-muted-foreground leading-relaxed">{s.bio}</p>
                    </CardContent>
                  )}
                </Card>
              ))}
            </div>
          </div>
        </section>
      )}

      <section ref={agendaRef} className="bg-background py-16 sm:py-20">
        <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
          <div className="text-center mb-10">
            <Badge variant="secondary" className="mb-3">Program</Badge>
            <h2 className="text-2xl font-bold tracking-tight sm:text-3xl">Conference Agenda</h2>
            <p className="mt-2 text-muted-foreground">Three full days of sessions across four thematic tracks</p>
          </div>

          {sessionsByDay.length > 0 ? (
            <>
              {/* Day tabs */}
              <div className="flex justify-center gap-2 mb-8">
                {sessionsByDay.map(({ day }, i) => (
                  <Button
                    key={day}
                    variant={i === activeDay ? "default" : "outline"}
                    size="sm"
                    onClick={() => setActiveDay(i)}
                  >
                    {day}
                  </Button>
                ))}
              </div>

              {/* Timeline */}
              <div className="max-w-3xl mx-auto">
                {sessionsByDay[activeDay]?.sessions.map((s, i) => {
                  const isBreak = s.track === "Break";
                  return (
                    <div key={s.id || i} className="flex gap-4 pb-8 last:pb-0">
                      {/* Time column */}
                      <div className="w-20 flex-shrink-0 text-right">
                        <p className="text-xs font-mono font-medium text-muted-foreground">
                          {formatTime(s.startTime)}
                        </p>
                        <p className="text-[10px] font-mono text-muted-foreground/60">
                          {formatTime(s.endTime)}
                        </p>
                      </div>

                      {/* Timeline line + dot */}
                      <div className="relative flex flex-col items-center">
                        <div className={`h-3 w-3 rounded-full border-2 flex-shrink-0 ${
                          isBreak ? "border-muted-foreground/50 bg-background" : "border-primary bg-primary"
                        }`} />
                        {i < (sessionsByDay[activeDay]?.sessions.length || 0) - 1 && (
                          <div className="flex-1 w-px bg-border mt-1" />
                        )}
                      </div>

                      {/* Session card */}
                      <div className={`flex-1 min-w-0 pb-2 ${isBreak ? "opacity-60" : ""}`}>
                        <div className="flex items-start justify-between gap-2">
                          <h4 className={`font-semibold text-sm ${isBreak ? "italic" : ""}`}>
                            {s.title}
                          </h4>
                          <Badge className={`text-[10px] flex-shrink-0 ${TRACK_COLORS[s.track] || "bg-muted text-muted-foreground"}`}>
                            {s.track}
                          </Badge>
                        </div>

                        {s.description && !isBreak && (
                          <p className="mt-1 text-xs text-muted-foreground">{s.description}</p>
                        )}

                        <div className="mt-1.5 flex flex-wrap items-center gap-x-3 gap-y-1 text-[11px] text-muted-foreground">
                          {s.location && (
                            <span className="flex items-center gap-1">
                              <MapPin className="h-3 w-3" /> {s.location}
                            </span>
                          )}
                          {s.speaker && (
                            <span className="flex items-center gap-1 font-medium text-foreground/70">
                              <Mic className="h-3 w-3" /> {s.speaker.name}
                            </span>
                          )}
                        </div>

                        {isArchived && s.isPublished && (
                          <div className="mt-2 flex flex-wrap gap-2">
                            {s.recordingUrl && (
                              <a href={s.recordingUrl} target="_blank" rel="noreferrer" className="inline-flex items-center gap-1 text-[11px] text-primary hover:underline">
                                <Video className="h-3 w-3" /> Recording
                              </a>
                            )}
                            {s.slideDeckUrl && (
                              <a href={s.slideDeckUrl} target="_blank" rel="noreferrer" className="inline-flex items-center gap-1 text-[11px] text-primary hover:underline">
                                <FileText className="h-3 w-3" /> Slides
                              </a>
                            )}
                          </div>
                        )}
                      </div>
                    </div>
                  );
                })}
              </div>
            </>
          ) : (
            <p className="text-center text-muted-foreground">Agenda coming soon. Check back for updates.</p>
          )}
        </div>
      </section>

      {speakers.length > 0 && (
        <section className="bg-muted/30 py-16 sm:py-20">
          <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
            <div className="text-center mb-10">
              <Badge variant="secondary" className="mb-3">All Speakers</Badge>
              <h2 className="text-2xl font-bold tracking-tight sm:text-3xl">Meet Our Faculty</h2>
            </div>
            <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
              {speakers.map((s) => (
                <Card key={s.id}>
                  <CardHeader className="flex-row items-center gap-3 space-y-0">
                    <div className="flex h-11 w-11 flex-shrink-0 items-center justify-center rounded-full bg-primary-soft text-sm font-semibold text-primary">
                      {getInitials(s.name)}
                    </div>
                    <div className="min-w-0">
                      <CardTitle className="text-sm">{s.name}</CardTitle>
                      <CardDescription className="text-xs">{s.title} · {s.organization}</CardDescription>
                    </div>
                  </CardHeader>
                </Card>
              ))}
            </div>
          </div>
        </section>
      )}

      <section className="bg-muted/30 py-16 sm:py-20">
        <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
          <div className="text-center mb-10">
            <Badge variant="secondary" className="mb-3">Partners</Badge>
            <h2 className="text-2xl font-bold tracking-tight sm:text-3xl">Our Partners</h2>
            <p className="mt-2 text-muted-foreground">Organizations we collaborate with to advance global health</p>
          </div>
          <PartnerCarousel />
        </div>
      </section>

      {conf.sponsors && conf.sponsors.length > 0 && (
        <section className="bg-background py-16 sm:py-20">
          <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
            <div className="text-center mb-10">
              <Badge variant="secondary" className="mb-3">Sponsors</Badge>
              <h2 className="text-2xl font-bold tracking-tight sm:text-3xl">Our Sponsors</h2>
              <p className="mt-2 text-muted-foreground">Thank you to our partners making this conference possible</p>
            </div>

            <div className="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-5 gap-4 max-w-3xl mx-auto">
              {conf.sponsors.map((s) => (
                <div key={s.id} className="flex items-center justify-center p-4 rounded-lg border bg-card hover:shadow-sm transition-shadow">
                  <div className="text-center">
                    <p className="font-semibold text-sm">{s.name}</p>
                  </div>
                </div>
              ))}
            </div>

            <p className="mt-10 text-center text-sm text-muted-foreground">
              Interested in sponsoring?{" "}
              <a href="mailto:info@gghn.org" className="text-primary hover:underline">Contact us</a>
            </p>
          </div>
        </section>
      )}

      <section className="bg-muted/30 py-16 sm:py-20">
        <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
          <div className="grid gap-10 lg:grid-cols-2 lg:items-center">
            <div>
              <Badge variant="secondary" className="mb-3">Venue</Badge>
              <h2 className="text-2xl font-bold tracking-tight sm:text-3xl">
                {conf.venue}
              </h2>
              <p className="mt-3 text-muted-foreground">
                The conference will be held at the Transcorp Hilton Abuja, Nigeria's premier conference
                destination. Located in the heart of the capital city, the venue offers world-class
                facilities and easy access to government institutions and diplomatic missions.
              </p>
              <div className="mt-4 space-y-2 text-sm">
                <p className="flex items-center gap-2 text-muted-foreground">
                  <MapPin className="h-4 w-4" /> 1 Aguiyi Ironsi Street, Maitama, Abuja
                </p>
                <p className="flex items-center gap-2 text-muted-foreground">
                  <ExternalLink className="h-4 w-4" /> Need a visa invitation letter? Contact us at info@gghn.org
                </p>
              </div>
            </div>
            <VenueMap />
          </div>
        </div>
      </section>

      {!isArchived && (
        <section className="bg-primary py-16 sm:py-20">
          <div className="mx-auto max-w-7xl px-4 text-center sm:px-6 lg:px-8">
            <h2 className="text-2xl font-bold tracking-tight text-primary-foreground sm:text-3xl">
              Ready to Join Us in Abuja?
            </h2>
            <p className="mt-3 text-primary-foreground/80 max-w-lg mx-auto">
              Secure your place at Nigeria's leading global health conference. Registration closes September 1, 2026.
            </p>
            <Button size="lg" variant="secondary" className="mt-6" onClick={() => setShowRegistration(true)}>
              Register Now
            </Button>
          </div>
        </section>
      )}

      <section className="bg-background py-16 sm:py-20">
        <div className="mx-auto max-w-3xl px-4 sm:px-6 lg:px-8">
          <div className="text-center mb-10">
            <Badge variant="secondary" className="mb-3">FAQ</Badge>
            <h2 className="text-2xl font-bold tracking-tight sm:text-3xl">Frequently Asked Questions</h2>
          </div>
          <Accordion type="single" collapsible className="w-full">
            {[
              { q: "How do I register for the conference?", a: "Click any 'Register Now' button on this page to complete the registration form. Your registration will be reviewed by our team, and you will receive a confirmation email upon approval." },
              { q: "Is there a registration fee?", a: "Registration is free for GGHN members and invited guests. A limited number of sponsored slots are available for participants from low-resource settings." },
              { q: "Will sessions be available online?", a: "Select plenary sessions will be livestreamed. Session recordings and slide decks will be available on this page after the conference." },
              { q: "What are the accommodation options?", a: "The Transcorp Hilton offers a discounted rate for conference attendees. Additional hotels are available within walking distance. Contact us for recommendations." },
              { q: "How do I get a visa invitation letter?", a: "Email info@gghn.org with your full name, passport number, and affiliation. We will provide an invitation letter to support your visa application." },
              { q: "Can I submit an abstract or present?", a: "The call for abstracts for GGHN 2026 has closed. The call for 2027 will open in early 2027. Subscribe to our newsletter for announcements." },
            ].map((faq, i) => (
              <AccordionItem key={i} value={`faq-${i}`}>
                <AccordionTrigger className="text-sm font-medium">{faq.q}</AccordionTrigger>
                <AccordionContent className="text-sm text-muted-foreground">{faq.a}</AccordionContent>
              </AccordionItem>
            ))}
          </Accordion>
        </div>
      </section>

      {/* Registration Modal */}
      <ConferenceRegistrationModal
        conferenceId={conf.id}
        conferenceTitle={conf.title}
        open={showRegistration}
        onClose={() => setShowRegistration(false)}
      />
    </SiteShell>
  );
}
