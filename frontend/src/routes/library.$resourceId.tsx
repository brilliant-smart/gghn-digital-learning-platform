import { createFileRoute, Link, notFound } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import { SiteShell } from "@/components/layout/SiteShell";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { ArrowLeft, ExternalLink } from "lucide-react";
import { resourceApi, type ResourceDto } from "@/api/resources";
import { discussionApi, type DiscussionDto } from "@/api/discussions";
import { DiscussionThread } from "@/components/DiscussionThread";
import { useAuth } from "@/lib/auth";

const audienceLabel = (a: string) =>
  a === "PolicyMakers" ? "Policy Makers" :
  a === "CommunityHealthWorkers" ? "Community Health Workers" : a;

export const Route = createFileRoute("/library/$resourceId")({
  head: () => ({
    meta: [
      { title: "Resource | GGHN Digital Learning" },
      { name: "description", content: "View resource details." },
    ],
  }),
  component: ResourceDetail,
  notFoundComponent: () => (
    <SiteShell>
      <div className="mx-auto max-w-3xl px-4 py-20 text-center">
        <h1 className="text-2xl font-bold">Resource not found</h1>
        <Link to="/library" className="mt-4 inline-block text-primary hover:underline">Back to library</Link>
      </div>
    </SiteShell>
  ),
});

function ResourceDetail() {
  const { resourceId } = Route.useParams();
  const { user, isAuthenticated } = useAuth();
  const [resource, setResource] = useState<ResourceDto | null>(null);
  const [related, setRelated] = useState<ResourceDto[]>([]);
  const [discussions, setDiscussions] = useState<DiscussionDto[]>([]);
  const [loading, setLoading] = useState(true);

  const fetchDiscussions = () => {
    discussionApi.getByResource(resourceId)
      .then((result) => setDiscussions(result.items))
      .catch(() => {});
  };

  useEffect(() => {
    resourceApi.getById(resourceId)
      .then(setResource)
      .catch(() => {})
      .finally(() => setLoading(false));

    resourceApi.getAll({ pageSize: 200 })
      .then((result) => setRelated(result.items.filter((r) => r.id !== resourceId).slice(0, 3)))
      .catch(() => {});

    fetchDiscussions();
  }, [resourceId]);

  if (loading) {
    return (
      <SiteShell>
        <div className="flex items-center justify-center py-20">
          <p className="text-muted-foreground">Loading...</p>
        </div>
      </SiteShell>
    );
  }

  if (!resource) {
    return (
      <SiteShell>
        <div className="mx-auto max-w-3xl px-4 py-20 text-center">
          <h1 className="text-2xl font-bold">Resource not found</h1>
          <Link to="/library" className="mt-4 inline-block text-primary hover:underline">Back to library</Link>
        </div>
      </SiteShell>
    );
  }

  return (
    <SiteShell>
      <article className="mx-auto max-w-4xl px-4 py-12 sm:px-6 lg:px-8">
        <Link to="/library" className="inline-flex items-center gap-1 text-sm text-muted-foreground hover:text-foreground">
          <ArrowLeft className="h-4 w-4" /> Back to library
        </Link>

        <div className="mt-6 flex flex-wrap items-center gap-2">
          <Badge variant="secondary" className="bg-primary-soft text-primary">{resource.topic}</Badge>
          <Badge variant="outline">{resource.difficulty}</Badge>
          <Badge variant="outline">{audienceLabel(resource.audience)}</Badge>
        </div>
        <h1 className="mt-4 text-3xl font-bold tracking-tight text-foreground sm:text-4xl">{resource.title}</h1>

        <div className="mt-6">
          <a href={resource.sourceUrl} target="_blank" rel="noreferrer">
            <Button>
              View Source <ExternalLink className="ml-2 h-4 w-4" />
            </Button>
          </a>
        </div>

        <section className="mt-10">
          <h2 className="text-lg font-semibold text-foreground">Plain-language summary</h2>
          <p className="mt-3 text-base leading-relaxed text-muted-foreground">
            {resource.plainLanguageSummary || resource.summary}
          </p>
        </section>

        {resource.takeaways && resource.takeaways.length > 0 && (
          <section className="mt-10 rounded-lg border border-border bg-muted/30 p-6">
            <h2 className="text-lg font-semibold text-foreground">Key takeaways</h2>
            <ul className="mt-4 space-y-3">
              {resource.takeaways.map((t: string, i: number) => (
                <li key={i} className="flex gap-3 text-sm text-foreground">
                  <span className="mt-1.5 h-1.5 w-1.5 flex-shrink-0 rounded-full bg-primary" />
                  <span>{t}</span>
                </li>
              ))}
            </ul>
          </section>
        )}

        <DiscussionThread
          resourceId={resourceId}
          discussions={discussions}
          onRefresh={fetchDiscussions}
          isAuthenticated={isAuthenticated}
          currentUserId={user?.id}
          isAdmin={user?.roles?.includes("Admin")}
        />

        {related.length > 0 && (
          <section className="mt-12">
            <h2 className="text-lg font-semibold text-foreground">Related resources</h2>
            <div className="mt-4 grid gap-4 md:grid-cols-3">
              {related.map((r) => (
                <Link key={r.id} to="/library/$resourceId" params={{ resourceId: r.id }}>
                  <Card className="h-full transition-all hover:shadow-[var(--shadow-elevated)]">
                    <CardHeader>
                      <CardTitle className="text-sm leading-snug">{r.title}</CardTitle>
                      <CardDescription className="line-clamp-2 text-xs">{r.plainLanguageSummary || r.summary}</CardDescription>
                    </CardHeader>
                    <CardContent>
                      <Badge variant="outline" className="text-[10px]">{r.difficulty}</Badge>
                    </CardContent>
                  </Card>
                </Link>
              ))}
            </div>
          </section>
        )}
      </article>
    </SiteShell>
  );
}