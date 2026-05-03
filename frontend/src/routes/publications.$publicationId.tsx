import { createFileRoute, Link, notFound } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import { SiteShell } from "@/components/layout/SiteShell";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { ArrowLeft, ExternalLink, Calendar, Users, Tag } from "lucide-react";
import { publicationApi, type PublicationDto } from "@/api/publications";

export const Route = createFileRoute("/publications/$publicationId")({
  head: () => ({
    meta: [
      { title: "Publication | GGHN Digital Learning" },
      { name: "description", content: "View publication details." },
    ],
  }),
  component: PublicationDetailPage,
});

function PublicationDetailPage() {
  const { publicationId } = Route.useParams();
  const [pub, setPub] = useState<PublicationDto | null>(null);
  const [related, setRelated] = useState<PublicationDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    publicationApi.getById(publicationId)
      .then((data) => setPub(data))
      .catch((err) => setError(err.message || "Publication not found"))
      .finally(() => setLoading(false));

    publicationApi.getAll({ pageSize: 100 })
      .then((result) => setRelated(result.items.filter((p: PublicationDto) => p.id !== publicationId).slice(0, 3)))
      .catch(() => {});
  }, [publicationId]);

  if (loading) {
    return (
      <SiteShell>
        <div className="flex items-center justify-center py-20">
          <p className="text-muted-foreground">Loading...</p>
        </div>
      </SiteShell>
    );
  }

  if (error || !pub) {
    return (
      <SiteShell>
        <div className="mx-auto max-w-3xl px-4 py-20 text-center sm:px-6">
          <h1 className="text-2xl font-semibold">Publication not found</h1>
          <p className="mt-2 text-muted-foreground">{error || "The publication you're looking for doesn't exist."}</p>
          <Link to="/publications" className="mt-6 inline-block">
            <Button>Back to Publications</Button>
          </Link>
        </div>
      </SiteShell>
    );
  }

  return (
    <SiteShell>
      <div className="mx-auto max-w-4xl px-4 py-10 sm:px-6 lg:px-8">
        <Link to="/publications" className="mb-6 inline-flex items-center gap-2 text-sm text-muted-foreground hover:text-foreground">
          <ArrowLeft className="h-4 w-4" /> Back to Publications
        </Link>

        <h1 className="text-3xl font-semibold leading-tight tracking-tight sm:text-4xl">{pub.title}</h1>

        <div className="mt-5 flex flex-wrap items-center gap-x-4 gap-y-2 text-sm text-muted-foreground">
          <div className="flex items-center gap-2">
            <Users className="h-4 w-4" />
            <span className="font-medium text-foreground">{pub.author}</span>
          </div>
          {pub.publicationType && (
            <Badge variant="secondary" className="bg-primary-soft text-primary">{pub.publicationType}</Badge>
          )}
          <div className="flex items-center gap-2">
            <Calendar className="h-4 w-4" />
            <span>{pub.publishedAt ? new Date(pub.publishedAt).toLocaleDateString() : new Date(pub.createdAt).toLocaleDateString()}</span>
          </div>
          {pub.year && (
            <span className="text-muted-foreground">{pub.year}</span>
          )}
        </div>

        {pub.tags && pub.tags.length > 0 && (
          <div className="mt-4 flex flex-wrap items-center gap-2">
            <Tag className="h-4 w-4 text-muted-foreground" />
            {pub.tags.map((tag) => (
              <Badge key={tag} variant="outline" className="text-xs">{tag}</Badge>
            ))}
          </div>
        )}

        <div className="mt-8 space-y-3">
          <h2 className="text-lg font-semibold">Summary</h2>
          <p className="text-muted-foreground leading-relaxed">{pub.summary}</p>
        </div>

        {pub.keyFindings && pub.keyFindings.length > 0 && (
          <section className="mt-10 rounded-lg border border-border bg-muted/30 p-6">
            <h2 className="text-lg font-semibold text-foreground">Key Findings</h2>
            <ul className="mt-4 space-y-3">
              {pub.keyFindings.map((finding: string, i: number) => (
                <li key={i} className="flex gap-3 text-sm text-foreground">
                  <span className="mt-1.5 h-1.5 w-1.5 flex-shrink-0 rounded-full bg-primary" />
                  <span>{finding}</span>
                </li>
              ))}
            </ul>
          </section>
        )}

        {pub.content && (
          <div className="mt-8 space-y-3">
            <h2 className="text-lg font-semibold">Full Content</h2>
            <div className="text-muted-foreground leading-relaxed whitespace-pre-line">{pub.content}</div>
          </div>
        )}

        {pub.externalUrl && (
          <div className="mt-8">
            <a href={pub.externalUrl} target="_blank" rel="noreferrer">
              <Button>
                View Publication <ExternalLink className="ml-2 h-4 w-4" />
              </Button>
            </a>
          </div>
        )}

        {related.length > 0 && (
          <div className="mt-14 border-t pt-10">
            <h2 className="mb-5 text-lg font-semibold">Related publications</h2>
            <div className="grid gap-4 md:grid-cols-3">
              {related.map((r) => (
                <Card key={r.id} className="flex h-full flex-col">
                  <CardHeader>
                    <CardTitle className="mt-2 text-base leading-snug">{r.title}</CardTitle>
                    <CardDescription className="line-clamp-2">{r.summary}</CardDescription>
                  </CardHeader>
                  <CardContent className="mt-auto">
                    <Link to="/publications/$publicationId" params={{ publicationId: r.id }}>
                      <Button size="sm" variant="outline">View</Button>
                    </Link>
                  </CardContent>
                </Card>
              ))}
            </div>
          </div>
        )}
      </div>
    </SiteShell>
  );
}