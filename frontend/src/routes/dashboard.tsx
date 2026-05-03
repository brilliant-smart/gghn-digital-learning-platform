import { createFileRoute, Link, useNavigate } from "@tanstack/react-router";
import { SiteShell } from "@/components/layout/SiteShell";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Progress } from "@/components/ui/progress";
import { BookOpen, CheckCircle2, TrendingUp } from "lucide-react";
import { useEffect, useState } from "react";
import { useAuth } from "@/lib/auth";
import { courseApi, type CourseDto } from "@/api/courses";
import { resourceApi, type ResourceDto } from "@/api/resources";
import { progressApi, type ProgressDto } from "@/api/progress";

export const Route = createFileRoute("/dashboard")({
  head: () => ({
    meta: [
      { title: "Dashboard | GGHN Digital Learning" },
      { name: "description", content: "Your personal GGHN learning dashboard." },
    ],
  }),
  component: Dashboard,
});

function Dashboard() {
  const { user, isAuthenticated, isLoading: authLoading } = useAuth();
  const navigate = useNavigate();
  const [courses, setCourses] = useState<CourseDto[]>([]);
  const [resources, setResources] = useState<ResourceDto[]>([]);
  const [progress, setProgress] = useState<ProgressDto[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!authLoading && !isAuthenticated) {
      navigate({ to: "/auth", replace: true });
      return;
    }
    if (isAuthenticated) {
      Promise.all([
        courseApi.getAll().catch(() => []),
        resourceApi.getAll({ pageSize: 100 }).catch(() => ({ items: [] })),
        progressApi.getMine().catch(() => []),
      ])
        .then(([coursesData, resourcesResult, progressData]) => {
          setCourses(coursesData as CourseDto[]);
          setResources((resourcesResult as any).items || []);
          setProgress(progressData as ProgressDto[]);
        })
        .finally(() => setLoading(false));
    }
  }, [authLoading, isAuthenticated]);

  if (authLoading || loading) {
    return (
      <SiteShell>
        <div className="flex items-center justify-center py-20">
          <p className="text-muted-foreground">Loading...</p>
        </div>
      </SiteShell>
    );
  }

  if (!isAuthenticated || !user) {
    return null;
  }

  const completedItems = progress.filter((p) => p.isCompleted).length;
  const totalProgress = progress.length;
  const avgProgress = totalProgress > 0
    ? Math.round(progress.filter((p) => p.isCompleted).length / totalProgress * 100)
    : 0;

  return (
    <SiteShell>
      <div className="border-b border-border bg-gradient-to-b from-muted/40 to-background">
        <div className="mx-auto max-w-7xl px-4 py-10 sm:px-6 lg:px-8">
          <p className="text-xs font-semibold uppercase tracking-wider text-primary">Dashboard</p>
          <h1 className="mt-2 text-3xl font-bold tracking-tight text-foreground sm:text-4xl">
            Welcome back, {user.firstName}
          </h1>
          <p className="mt-2 text-muted-foreground">Continue building expertise in global public health.</p>
        </div>
      </div>

      <div className="mx-auto max-w-7xl space-y-10 px-4 py-10 sm:px-6 lg:px-8">
        {/* Stats */}
        <div className="grid gap-4 sm:grid-cols-3">
          <StatCard icon={BookOpen} label="Courses available" value={courses.length.toString()} />
          <StatCard icon={CheckCircle2} label="Completed items" value={completedItems.toString()} />
          <StatCard icon={TrendingUp} label="Avg. progress" value={`${avgProgress}%`} />
        </div>

        {/* Courses */}
        <section>
          <h2 className="text-xl font-semibold tracking-tight text-foreground">Courses</h2>
          <div className="mt-5 grid gap-5 md:grid-cols-2">
            {courses.slice(0, 4).map((c) => {
              const courseProgress = progress.filter(
                (p) => p.courseId === c.id && p.isCompleted
              ).length;
              const lessonCount = c.lessons.length;
              const pct = lessonCount > 0 ? Math.round((courseProgress / lessonCount) * 100) : 0;

              return (
                <Card key={c.id}>
                  <CardHeader>
                    <Badge variant="secondary" className="w-fit bg-primary-soft text-primary">{c.topic}</Badge>
                    <CardTitle className="mt-3 text-base leading-snug">{c.title}</CardTitle>
                    <CardDescription className="line-clamp-2">{c.description}</CardDescription>
                  </CardHeader>
                  <CardContent className="space-y-4">
                    <div>
                      <div className="mb-1.5 flex items-center justify-between text-xs text-muted-foreground">
                        <span>{pct}% complete</span>
                        <span>{courseProgress}/{lessonCount} lessons</span>
                      </div>
                      <Progress value={pct} />
                    </div>
                    <Link to="/learning/$courseId" params={{ courseId: c.id }}>
                      <Button size="sm">{pct > 0 ? "Continue" : "Start course"}</Button>
                    </Link>
                  </CardContent>
                </Card>
              );
            })}
          </div>
        </section>

        {/* Recommended */}
        <section>
          <h2 className="text-xl font-semibold tracking-tight text-foreground">Recommended for you</h2>
          <div className="mt-5 grid gap-5 md:grid-cols-2 lg:grid-cols-3">
            {resources.slice(0, 3).map((r) => (
              <Link key={r.id} to="/library/$resourceId" params={{ resourceId: r.id }}>
                <Card className="h-full transition-all hover:shadow-[var(--shadow-elevated)]">
                  <CardHeader>
                    <Badge variant="outline" className="w-fit">{r.topic}</Badge>
                    <CardTitle className="mt-3 text-base leading-snug">{r.title}</CardTitle>
                    <CardDescription className="line-clamp-3">{r.plainLanguageSummary || r.summary}</CardDescription>
                  </CardHeader>
                </Card>
              </Link>
            ))}
          </div>
        </section>
      </div>
    </SiteShell>
  );
}

function StatCard({ icon: Icon, label, value }: { icon: React.ElementType; label: string; value: string }) {
  return (
    <Card>
      <CardContent className="flex items-center gap-4 p-5">
        <div className="flex h-11 w-11 items-center justify-center rounded-md bg-primary-soft text-primary">
          <Icon className="h-5 w-5" />
        </div>
        <div>
          <p className="text-2xl font-bold tracking-tight text-foreground">{value}</p>
          <p className="text-xs text-muted-foreground">{label}</p>
        </div>
      </CardContent>
    </Card>
  );
}