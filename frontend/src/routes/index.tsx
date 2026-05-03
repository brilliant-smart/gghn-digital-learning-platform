import { createFileRoute, Link } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import { SiteShell } from "@/components/layout/SiteShell";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { ArrowRight, BookOpen, Calendar, FileText, GraduationCap } from "lucide-react";
import { pathwayApi, type PathwayDto } from "@/api/pathways";
import { resourceApi, type ResourceDto } from "@/api/resources";
import { publicationApi, type PublicationDto } from "@/api/publications";

export const Route = createFileRoute("/")({
  head: () => ({
    meta: [
      { title: "GGHN Digital Learning Platform" },
      { name: "description", content: "Strengthening health systems through knowledge, training, and research-driven learning." },
      { property: "og:title", content: "GGHN Digital Learning Platform" },
      { property: "og:description", content: "Strengthening health systems through knowledge, training, and research-driven learning." },
    ],
  }),
  component: Index,
});

function Index() {
  const [pathways, setPathways] = useState<PathwayDto[]>([]);
  const [resources, setResources] = useState<ResourceDto[]>([]);
  const [publications, setPublications] = useState<PublicationDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    Promise.all([
      pathwayApi.getAll(),
      resourceApi.getAll({ pageSize: 100 }),
      publicationApi.getAll({ pageSize: 6 }),
    ])
      .then(([pathwaysData, resourcesResult, publicationsResult]) => {
        setPathways(pathwaysData);
        setResources(resourcesResult.items);
        setPublications(publicationsResult.items);
      })
      .catch((err) => setError(err.message || "Failed to load data"))
      .finally(() => setLoading(false));
  }, []);

  if (loading) {
    return (
      <SiteShell>
        <div className="flex items-center justify-center py-20">
          <p className="text-muted-foreground">Loading...</p>
        </div>
      </SiteShell>
    );
  }

  if (error) {
    return (
      <SiteShell>
        <div className="flex items-center justify-center py-20">
          <p className="text-destructive">{error}</p>
        </div>
      </SiteShell>
    );
  }

  return (
    <SiteShell>
      {/* Hero */}
      <section
        className="relative overflow-hidden"
        style={{ backgroundImage: "var(--gradient-hero)" }}
      >
        {/* Decorative blobs */}
        <div className="pointer-events-none absolute -right-24 -top-24 h-96 w-96 rounded-full bg-white/10 blur-3xl" />
        <div className="pointer-events-none absolute -bottom-32 left-1/3 h-96 w-96 rounded-full bg-white/5 blur-3xl" />
        <div className="relative mx-auto grid max-w-7xl gap-12 px-4 py-20 sm:px-6 lg:grid-cols-[1.1fr_1fr] lg:items-center lg:px-8 lg:py-28">
          <div>
            <Badge className="border-0 bg-white/15 text-white hover:bg-white/20">
              Georgetown Global Health Nigeria
            </Badge>
            <h1 className="mt-5 text-4xl font-bold tracking-tight text-white sm:text-5xl lg:text-6xl">
              GGHN Digital Learning Platform
            </h1>
            <p className="mt-5 max-w-xl text-base leading-relaxed text-white/85 sm:text-lg">
              Strengthening health systems through knowledge, training, and research-driven learning for clinicians, researchers, and policy makers across Africa.
            </p>
            <div className="mt-8 flex flex-wrap gap-3">
              <Link to="/library">
                <Button size="lg" className="bg-white text-primary hover:bg-white/90">
                  Explore Library <ArrowRight className="ml-2 h-4 w-4" />
                </Button>
              </Link>
              <Link to="/learning">
                <Button size="lg" variant="outline" className="border-white/30 bg-white/10 text-white hover:bg-white/20">
                  Browse Courses
                </Button>
              </Link>
            </div>
          </div>

          {/* Right visual: floating dashboard preview */}
          <div className="relative hidden lg:block">
            <div className="absolute inset-0 rounded-2xl bg-white/10 blur-2xl" />
            <div className="relative rounded-2xl border border-white/20 bg-white/10 p-5 shadow-2xl backdrop-blur-md">
              <div className="flex items-center gap-2 pb-4">
                <span className="h-2.5 w-2.5 rounded-full bg-white/40" />
                <span className="h-2.5 w-2.5 rounded-full bg-white/30" />
                <span className="h-2.5 w-2.5 rounded-full bg-white/20" />
                <span className="ml-2 text-xs text-white/70">Your learning</span>
              </div>
              <div className="space-y-3">
                {pathways.slice(0, 3).map((c) => (
                  <div key={c.id} className="rounded-lg bg-white/95 p-3.5 text-foreground shadow-sm">
                    <div className="flex items-center gap-2.5">
                      <span className="flex h-8 w-8 items-center justify-center rounded-md bg-primary-soft text-primary">
                        <GraduationCap className="h-4 w-4" />
                      </span>
                      <div className="min-w-0 flex-1">
                        <p className="truncate text-sm font-medium">{c.title}</p>
                        <div className="mt-1.5 h-1.5 w-full overflow-hidden rounded-full bg-muted">
                          <div className="h-full rounded-full bg-primary" style={{ width: `${Math.min(c.resourceCount * 8, 100)}%` }} />
                        </div>
                      </div>
                      <span className="text-xs font-medium text-muted-foreground">{c.resourceCount}</span>
                    </div>
                  </div>
                ))}
              </div>
              <div className="mt-4 grid grid-cols-3 gap-2 text-center">
                {[
                  { v: String(pathways.length), l: "Pathways" },
                  { v: String(resources.length), l: "Resources" },
                  { v: String(publications.length), l: "Publications" },
                ].map((s) => (
                  <div key={s.l} className="rounded-md bg-white/10 px-2 py-2 text-white">
                    <p className="text-base font-semibold">{s.v}</p>
                    <p className="text-[10px] text-white/70">{s.l}</p>
                  </div>
                ))}
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* Stats */}
      <section className="border-b border-border bg-muted/30">
        <div className="mx-auto grid max-w-7xl grid-cols-2 gap-6 px-4 py-10 sm:px-6 md:grid-cols-4 lg:px-8">
          {[
            { label: "Resources", value: String(resources.length) },
            { label: "Courses", value: "35" },
            { label: "Learning Pathways", value: String(pathways.length) },
            { label: "Trained Professionals", value: "8,500+" },
          ].map((s) => (
            <div key={s.label}>
              <p className="text-3xl font-bold tracking-tight text-foreground">{s.value}</p>
              <p className="mt-1 text-sm text-muted-foreground">{s.label}</p>
            </div>
          ))}
        </div>
      </section>

      {/* Pathways */}
      <section className="mx-auto max-w-7xl px-4 py-16 sm:px-6 lg:px-8">
        <SectionHeading
          eyebrow="Featured Learning Pathways"
          title="Curated journeys for public health professionals"
          link={{ to: "/pathways", label: "All pathways" }}
        />
        <div className="mt-8 grid gap-6 md:grid-cols-2 lg:grid-cols-3">
          {pathways.slice(0, 3).map((p) => (
            <Card key={p.id} className="group transition-all hover:shadow-[var(--shadow-elevated)]">
              <CardHeader>
                <div className="flex h-10 w-10 items-center justify-center rounded-md bg-primary-soft text-primary">
                  <GraduationCap className="h-5 w-5" />
                </div>
                <CardTitle className="mt-3 text-lg">{p.title}</CardTitle>
                <CardDescription>{p.description}</CardDescription>
              </CardHeader>
              <CardContent>
                <div className="flex items-center justify-between">
                  <span className="text-xs text-muted-foreground">{p.resourceCount} resources</span>
                  <Link to="/pathways">
                    <Button variant="ghost" size="sm">Start Pathway <ArrowRight className="ml-1 h-3.5 w-3.5" /></Button>
                  </Link>
                </div>
              </CardContent>
            </Card>
          ))}
        </div>
      </section>

      {/* Featured Resources */}
      <section className="bg-muted/30">
        <div className="mx-auto max-w-7xl px-4 py-16 sm:px-6 lg:px-8">
          <SectionHeading
            eyebrow="Featured Resources"
            title="Research summaries and policy briefs"
            link={{ to: "/library", label: "Browse library" }}
          />
          <div className="mt-8 grid gap-6 md:grid-cols-2">
            {resources.slice(0, 4).map((r) => (
              <Link key={r.id} to="/library/$resourceId" params={{ resourceId: r.id }}>
                <Card className="h-full transition-all hover:shadow-[var(--shadow-elevated)]">
                  <CardHeader>
                    <div className="flex items-center gap-2">
                      <Badge variant="secondary" className="bg-primary-soft text-primary">{r.topic}</Badge>
                      <Badge variant="outline">{r.difficulty}</Badge>
                    </div>
                    <CardTitle className="mt-3 text-lg leading-snug">{r.title}</CardTitle>
                    <CardDescription className="line-clamp-3">{r.plainLanguageSummary || r.summary}</CardDescription>
                  </CardHeader>
                </Card>
              </Link>
            ))}
          </div>
        </div>
      </section>

      {/* Featured Publications */}
      {publications.length > 0 && (
        <section className="mx-auto max-w-7xl px-4 py-16 sm:px-6 lg:px-8">
          <SectionHeading
            eyebrow="Latest Publications"
            title="Research, reports, and policy insights"
            link={{ to: "/publications", label: "All publications" }}
          />
          <div className="mt-8 grid gap-6 md:grid-cols-2 lg:grid-cols-3">
            {publications.slice(0, 3).map((p) => (
              <Card key={p.id} className="group transition-all hover:shadow-[var(--shadow-elevated)]">
                <CardHeader>
                  <div className="flex items-center gap-2">
                    <Badge variant="secondary" className="bg-primary-soft text-primary">Publication</Badge>
                  </div>
                  <CardTitle className="mt-3 text-lg leading-snug">{p.title}</CardTitle>
                  <CardDescription className="line-clamp-3">{p.summary}</CardDescription>
                </CardHeader>
                <CardContent>
                  <div className="flex items-center justify-between">
                    <span className="text-xs text-muted-foreground">{p.author}</span>
                    <Link to="/publications/$publicationId" params={{ publicationId: p.id }}>
                      <Button variant="ghost" size="sm">Read more <ArrowRight className="ml-1 h-3.5 w-3.5" /></Button>
                    </Link>
                  </div>
                </CardContent>
              </Card>
            ))}
          </div>
        </section>
      )}

      {/* Upcoming */}
      <section className="bg-muted/30">
        <div className="mx-auto max-w-7xl px-4 py-16 sm:px-6 lg:px-8">
          <SectionHeading
            eyebrow="Upcoming"
            title="Trainings, webinars & workshops"
            link={{ to: "/conference", label: "View conference" }}
          />
          <div className="mt-8 grid gap-6 md:grid-cols-3">
            {[
              { icon: Calendar, title: "Annual GGHN Public Health Conference", date: "Sep 15–17, 2026", tag: "Conference" },
              { icon: BookOpen, title: "Webinar: Disease Surveillance in Practice", date: "Oct 22, 2026", tag: "Webinar" },
              { icon: FileText, title: "Workshop: M&E Framework Design", date: "Nov 5, 2026", tag: "Workshop" },
            ].map((e) => (
              <Card key={e.title}>
                <CardHeader>
                  <div className="flex items-center gap-2">
                    <e.icon className="h-4 w-4 text-secondary" />
                    <Badge variant="outline">{e.tag}</Badge>
                  </div>
                  <CardTitle className="mt-2 text-base leading-snug">{e.title}</CardTitle>
                  <CardDescription>{e.date}</CardDescription>
                </CardHeader>
              </Card>
            ))}
          </div>
        </div>
      </section>
    </SiteShell>
  );
}

function SectionHeading({
  eyebrow,
  title,
  link,
}: {
  eyebrow: string;
  title: string;
  link?: { to: string; label: string };
}) {
  return (
    <div className="flex items-end justify-between gap-4">
      <div>
        <p className="text-xs font-semibold uppercase tracking-wider text-primary">{eyebrow}</p>
        <h2 className="mt-2 text-2xl font-bold tracking-tight text-foreground sm:text-3xl">{title}</h2>
      </div>
      {link && (
        <Link to={link.to} className="hidden text-sm font-medium text-primary hover:underline sm:inline">
          {link.label} →
        </Link>
      )}
    </div>
  );
}