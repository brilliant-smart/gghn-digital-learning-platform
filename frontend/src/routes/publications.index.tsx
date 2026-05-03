import { createFileRoute, Link } from "@tanstack/react-router";
import { useMemo, useState, useEffect } from "react";
import { SiteShell } from "@/components/layout/SiteShell";
import { PageHeader } from "@/components/PageHeader";
import { Input } from "@/components/ui/input";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Search, Calendar, Users } from "lucide-react";
import { publicationApi, type PublicationDto } from "@/api/publications";

const PAGE_SIZE = 6;

export const Route = createFileRoute("/publications/")({
  head: () => ({
    meta: [
      { title: "Publications | GGHN Digital Learning" },
      { name: "description", content: "Research, reports, and insights from GGHN and partners advancing global health systems." },
      { property: "og:title", content: "Publications | GGHN Digital Learning" },
      { property: "og:description", content: "Research, reports, and insights from GGHN and partners advancing global health systems." },
    ],
  }),
  component: PublicationsPage,
});

const PUBLICATION_TYPES = ["Journal Article", "Report", "Policy Brief", "Conference Abstract"];

function PublicationsPage() {
  const [q, setQ] = useState("");
  const [visible, setVisible] = useState(PAGE_SIZE);
  const [publications, setPublications] = useState<PublicationDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    publicationApi.getAll({ pageSize: 100 })
      .then((result) => setPublications(result.items))
      .catch((err) => setError(err.message || "Failed to load publications"))
      .finally(() => setLoading(false));
  }, []);

  const filtered = useMemo(() => {
    if (!q) return publications;
    const qLower = q.toLowerCase();
    return publications.filter((p) =>
      p.title.toLowerCase().includes(qLower) ||
      p.author.toLowerCase().includes(qLower) ||
      p.summary.toLowerCase().includes(qLower)
    );
  }, [q, publications]);

  const visibleList = filtered.slice(0, visible);

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
      <PageHeader
        eyebrow="Publications"
        title="Publications"
        description="Research, reports, and insights from GGHN and partners advancing global health systems."
      />
      <div className="mx-auto max-w-7xl px-4 py-10 sm:px-6 lg:px-8">
        <div className="mb-8">
          <div className="relative w-full max-w-xl">
            <Search className="pointer-events-none absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground" />
            <Input
              placeholder="Search by title, keyword, or author..."
              value={q}
              onChange={(e) => { setQ(e.target.value); setVisible(PAGE_SIZE); }}
              className="pl-9"
            />
          </div>
        </div>

        <div>
          <p className="mb-4 text-sm text-muted-foreground">
            Showing {Math.min(visibleList.length, filtered.length)} of {filtered.length} publications
          </p>
          <div className="grid gap-5 md:grid-cols-2">
            {visibleList.map((p) => (
              <Card key={p.id} className="flex h-full flex-col transition-shadow hover:shadow-md">
                <CardHeader>
                  <CardTitle className="mt-3 text-lg leading-snug">{p.title}</CardTitle>
                  <CardDescription className="line-clamp-3">{p.summary}</CardDescription>
                </CardHeader>
                <CardContent className="mt-auto space-y-3">
                  <div className="flex items-center gap-2 text-xs text-muted-foreground">
                    <Users className="h-3.5 w-3.5" />
                    <span>{p.author}</span>
                  </div>
                  <div className="flex items-center gap-2 text-xs text-muted-foreground">
                    <Calendar className="h-3.5 w-3.5" />
                    <span>{p.publishedAt ? new Date(p.publishedAt).toLocaleDateString() : new Date(p.createdAt).toLocaleDateString()}</span>
                  </div>
                  <Link to="/publications/$publicationId" params={{ publicationId: p.id }}>
                    <Button size="sm">View Details</Button>
                  </Link>
                </CardContent>
              </Card>
            ))}
            {filtered.length === 0 && (
              <p className="col-span-full py-12 text-center text-sm text-muted-foreground">
                No publications match your search.
              </p>
            )}
          </div>
          {visible < filtered.length && (
            <div className="mt-8 flex justify-center">
              <Button variant="outline" onClick={() => setVisible((v) => v + PAGE_SIZE)}>
                Load more
              </Button>
            </div>
          )}
        </div>
      </div>
    </SiteShell>
  );
}