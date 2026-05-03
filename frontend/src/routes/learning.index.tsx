import { createFileRoute, Link } from "@tanstack/react-router";
import { useMemo, useState, useEffect } from "react";
import { SiteShell } from "@/components/layout/SiteShell";
import { PageHeader } from "@/components/PageHeader";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Progress } from "@/components/ui/progress";
import { courseApi, type CourseDto } from "@/api/courses";

const TOPICS = [
  "Health Systems Strengthening",
  "Infectious Disease Control",
  "HIV/AIDS Programs",
  "Maternal & Child Health",
  "Disease Surveillance",
  "Community Health",
  "Public Health Policy",
];

export const Route = createFileRoute("/learning/")({
  head: () => ({
    meta: [
      { title: "Courses | GGHN Digital Learning" },
      { name: "description", content: "Online courses for public health professionals across Africa." },
      { property: "og:title", content: "Courses | GGHN Digital Learning" },
      { property: "og:description", content: "Online courses for public health professionals across Africa." },
    ],
  }),
  component: LearningPage,
});

function LearningPage() {
  const [topic, setTopic] = useState<string>("All");
  const [courses, setCourses] = useState<CourseDto[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    courseApi.getAll()
      .then(setCourses)
      .catch(() => {})
      .finally(() => setLoading(false));
  }, []);

  const filtered = useMemo(
    () => (topic === "All" ? courses : courses.filter((c) => c.topic === topic)),
    [topic, courses]
  );

  const filters = ["All", ...TOPICS.filter((t) => courses.some((c) => c.topic === t))];

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
        eyebrow="Learning"
        title="Courses & training"
        description="Self-paced courses developed with public health practitioners and academic experts."
      />
      <div className="mx-auto max-w-7xl px-4 py-10 sm:px-6 lg:px-8">
        <div className="mb-8 flex flex-wrap gap-2">
          {filters.map((t) => (
            <button
              key={t}
              onClick={() => setTopic(t)}
              className={`rounded-full border px-3 py-1.5 text-xs font-medium transition-colors ${
                topic === t
                  ? "border-primary bg-primary text-primary-foreground"
                  : "border-border bg-background text-muted-foreground hover:border-primary/40 hover:text-foreground"
              }`}
            >
              {t}
            </button>
          ))}
        </div>

        <div className="grid gap-6 md:grid-cols-2 lg:grid-cols-3">
          {filtered.map((c) => (
            <Card key={c.id} className="flex h-full flex-col">
              <CardHeader>
                <Badge variant="secondary" className="w-fit bg-primary-soft text-primary">{c.topic}</Badge>
                <CardTitle className="mt-3 text-lg leading-snug">{c.title}</CardTitle>
                <CardDescription className="line-clamp-3">{c.description}</CardDescription>
              </CardHeader>
              <CardContent className="mt-auto space-y-4">
                <div className="flex items-center gap-2 text-xs text-muted-foreground">
                  <span>{c.difficulty}</span>
                  <span>·</span>
                  <span>{c.durationMinutes} min</span>
                  <span>·</span>
                  <span>{c.lessons.length} lessons</span>
                </div>
                <Link to="/learning/$courseId" params={{ courseId: c.id }}>
                  <Button size="sm" className="w-full">Start course</Button>
                </Link>
              </CardContent>
            </Card>
          ))}
        </div>
      </div>
    </SiteShell>
  );
}