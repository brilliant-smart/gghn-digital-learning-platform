import { createFileRoute, Link } from "@tanstack/react-router";
import { useMemo, useState, useEffect } from "react";
import { SiteShell } from "@/components/layout/SiteShell";
import { PageHeader } from "@/components/PageHeader";
import { Input } from "@/components/ui/input";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Checkbox } from "@/components/ui/checkbox";
import { Label } from "@/components/ui/label";
import { Search } from "lucide-react";
import { resourceApi, type ResourceDto } from "@/api/resources";

const TOPICS = [
  "Health Systems Strengthening",
  "Infectious Disease Control",
  "HIV/AIDS Programs",
  "Maternal & Child Health",
  "Disease Surveillance",
  "Community Health",
  "Public Health Policy",
];
const AUDIENCES = ["PolicyMakers", "Clinicians", "Researchers", "CommunityHealthWorkers"];
const DIFFICULTIES = ["Basic", "Intermediate", "Advanced"];

const audienceLabel = (a: string) =>
  a === "PolicyMakers" ? "Policy Makers" :
  a === "CommunityHealthWorkers" ? "Community Health Workers" : a;

export const Route = createFileRoute("/library/")({
  head: () => ({
    meta: [
      { title: "Library | GGHN Digital Learning" },
      { name: "description", content: "Search GGHN's curated public health resources, policy briefs, and research summaries." },
      { property: "og:title", content: "Library | GGHN Digital Learning" },
      { property: "og:description", content: "Search GGHN's curated public health resources, policy briefs, and research summaries." },
    ],
  }),
  component: LibraryPage,
});

function LibraryPage() {
  const [q, setQ] = useState("");
  const [topics, setTopics] = useState<string[]>([]);
  const [audiences, setAudiences] = useState<string[]>([]);
  const [difficulties, setDifficulties] = useState<string[]>([]);
  const [allResources, setAllResources] = useState<ResourceDto[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    resourceApi.getAll({ pageSize: 200 })
      .then((result) => setAllResources(result.items))
      .catch(() => {})
      .finally(() => setLoading(false));
  }, []);

  const filtered = useMemo(() => {
    return allResources.filter((r) => {
      if (q && !`${r.title} ${r.summary} ${r.plainLanguageSummary}`.toLowerCase().includes(q.toLowerCase())) return false;
      if (topics.length && !topics.includes(r.topic)) return false;
      if (audiences.length && !audiences.includes(r.audience)) return false;
      if (difficulties.length && !difficulties.includes(r.difficulty)) return false;
      return true;
    });
  }, [q, topics, audiences, difficulties, allResources]);

  const toggle = (list: string[], setList: (v: string[]) => void, v: string) =>
    setList(list.includes(v) ? list.filter((x) => x !== v) : [...list, v]);

  if (loading) {
    return (
      <SiteShell>
        <div className="flex items-center justify-center py-20">
          <p className="text-muted-foreground">Loading...</p>
        </div>
      </SiteShell>
    );
  }

  return (
    <SiteShell>
      <PageHeader
        eyebrow="Digital Library"
        title="Public health resources & research"
        description="Plain-language summaries of evidence to inform practice and policy across Africa."
      />
      <div className="mx-auto max-w-7xl px-4 py-10 sm:px-6 lg:px-8">
        <div className="relative mb-8 max-w-xl">
          <Search className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
          <Input
            placeholder="Search resources..."
            value={q}
            onChange={(e) => setQ(e.target.value)}
            className="pl-9"
          />
        </div>

        <div className="grid gap-8 lg:grid-cols-[260px_1fr]">
          <aside className="space-y-6">
            <FilterGroup
              title="Topic"
              options={TOPICS}
              selected={topics}
              onToggle={(v) => toggle(topics, setTopics, v)}
            />
            <FilterGroup
              title="Audience"
              options={AUDIENCES}
              selected={audiences}
              onToggle={(v) => toggle(audiences, setAudiences, v)}
              displayFn={audienceLabel}
            />
            <FilterGroup
              title="Difficulty"
              options={DIFFICULTIES}
              selected={difficulties}
              onToggle={(v) => toggle(difficulties, setDifficulties, v)}
            />
          </aside>

          <div>
            <p className="mb-4 text-sm text-muted-foreground">{filtered.length} resources</p>
            <div className="grid gap-5 md:grid-cols-2">
              {filtered.map((r) => (
                <Card key={r.id} className="flex h-full flex-col">
                  <CardHeader>
                    <div className="flex flex-wrap items-center gap-2">
                      <Badge variant="secondary" className="bg-primary-soft text-primary">{r.topic}</Badge>
                      <Badge variant="outline">{r.difficulty}</Badge>
                      <Badge variant="outline">{audienceLabel(r.audience)}</Badge>
                    </div>
                    <CardTitle className="mt-3 text-lg leading-snug">{r.title}</CardTitle>
                    <CardDescription className="line-clamp-3">{r.plainLanguageSummary || r.summary}</CardDescription>
                  </CardHeader>
                  <CardContent className="mt-auto">
                    <Link to="/library/$resourceId" params={{ resourceId: r.id }}>
                      <Button size="sm">View Resource</Button>
                    </Link>
                  </CardContent>
                </Card>
              ))}
              {filtered.length === 0 && (
                <p className="col-span-full py-12 text-center text-sm text-muted-foreground">
                  No resources match your filters.
                </p>
              )}
            </div>
          </div>
        </div>
      </div>
    </SiteShell>
  );
}

function FilterGroup({
  title,
  options,
  selected,
  onToggle,
  displayFn,
}: {
  title: string;
  options: readonly string[];
  selected: string[];
  onToggle: (v: string) => void;
  displayFn?: (v: string) => string;
}) {
  return (
    <div>
      <p className="mb-3 text-xs font-semibold uppercase tracking-wider text-foreground">{title}</p>
      <div className="space-y-2">
        {options.map((o) => {
          const id = `${title}-${o}`;
          return (
            <div key={o} className="flex items-center gap-2">
              <Checkbox id={id} checked={selected.includes(o)} onCheckedChange={() => onToggle(o)} />
              <Label htmlFor={id} className="cursor-pointer text-sm font-normal text-muted-foreground">
                {displayFn ? displayFn(o) : o}
              </Label>
            </div>
          );
        })}
      </div>
    </div>
  );
}