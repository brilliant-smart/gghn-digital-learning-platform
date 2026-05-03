import { createFileRoute, Link, notFound } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import { SiteShell } from "@/components/layout/SiteShell";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Progress } from "@/components/ui/progress";
import { ArrowLeft, CheckCircle2, Circle, PlayCircle } from "lucide-react";
import { courseApi, type CourseDto } from "@/api/courses";
import { progressApi } from "@/api/progress";
import { useAuth } from "@/lib/auth";

export const Route = createFileRoute("/learning/$courseId")({
  head: () => ({
    meta: [
      { title: "Course | GGHN Digital Learning" },
      { name: "description", content: "View course details and lessons." },
    ],
  }),
  component: CourseDetail,
  notFoundComponent: () => (
    <SiteShell>
      <div className="mx-auto max-w-3xl px-4 py-20 text-center">
        <h1 className="text-2xl font-bold">Course not found</h1>
        <Link to="/learning" className="mt-4 inline-block text-primary hover:underline">Back to courses</Link>
      </div>
    </SiteShell>
  ),
});

function CourseDetail() {
  const { courseId } = Route.useParams();
  const { isAuthenticated } = useAuth();
  const [course, setCourse] = useState<CourseDto | null>(null);
  const [completedLessons, setCompletedLessons] = useState<Set<string>>(new Set());
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    courseApi.getById(courseId)
      .then(setCourse)
      .catch(() => {})
      .finally(() => setLoading(false));

    if (isAuthenticated) {
      progressApi.getMine()
        .then((data) => {
          const completed = new Set(
            data.filter((p) => p.courseId === courseId && p.isCompleted && p.lessonId).map((p) => p.lessonId!)
          );
          setCompletedLessons(completed);
        })
        .catch(() => {});
    }
  }, [courseId, isAuthenticated]);

  if (loading) {
    return (
      <SiteShell>
        <div className="flex items-center justify-center py-20">
          <p className="text-muted-foreground">Loading...</p>
        </div>
      </SiteShell>
    );
  }

  if (!course) {
    return (
      <SiteShell>
        <div className="mx-auto max-w-3xl px-4 py-20 text-center">
          <h1 className="text-2xl font-bold">Course not found</h1>
          <Link to="/learning" className="mt-4 inline-block text-primary hover:underline">Back to courses</Link>
        </div>
      </SiteShell>
    );
  }

  const progressPct = course.lessons.length > 0
    ? Math.round((completedLessons.size / course.lessons.length) * 100)
    : 0;

  const handleMarkComplete = async (lessonId: string) => {
    if (!isAuthenticated) return;
    try {
      await progressApi.markLessonComplete(lessonId, courseId);
      setCompletedLessons((prev) => new Set(prev).add(lessonId));
    } catch {}
  };

  return (
    <SiteShell>
      <article className="mx-auto max-w-5xl px-4 py-12 sm:px-6 lg:px-8">
        <Link to="/learning" className="inline-flex items-center gap-1 text-sm text-muted-foreground hover:text-foreground">
          <ArrowLeft className="h-4 w-4" /> All courses
        </Link>

        <Badge variant="secondary" className="mt-6 bg-primary-soft text-primary">{course.topic}</Badge>
        <h1 className="mt-3 text-3xl font-bold tracking-tight text-foreground sm:text-4xl">{course.title}</h1>
        <p className="mt-3 max-w-3xl text-muted-foreground">{course.description}</p>

        <div className="mt-6 max-w-md">
          <div className="mb-2 flex items-center justify-between text-sm">
            <span className="font-medium text-foreground">Course progress</span>
            <span className="text-muted-foreground">{progressPct}%</span>
          </div>
          <Progress value={progressPct} />
        </div>

        {/* Video placeholder */}
        <div className="mt-10 flex aspect-video items-center justify-center rounded-lg border border-border bg-muted">
          <div className="text-center">
            <PlayCircle className="mx-auto h-14 w-14 text-muted-foreground" />
            <p className="mt-3 text-sm text-muted-foreground">Training video player (embed-ready)</p>
          </div>
        </div>

        <div className="mt-10 grid gap-8 lg:grid-cols-[1fr_300px]">
          <section>
            <h2 className="text-lg font-semibold text-foreground">Lessons</h2>
            <ul className="mt-4 divide-y divide-border rounded-lg border border-border bg-card">
              {course.lessons.map((l, i) => {
                const isCompleted = completedLessons.has(l.id);
                return (
                  <li key={l.id} className="flex items-center justify-between p-4">
                    <div className="flex items-center gap-3">
                      {isCompleted ? (
                        <CheckCircle2 className="h-5 w-5 text-secondary" />
                      ) : (
                        <Circle className="h-5 w-5 text-muted-foreground" />
                      )}
                      <div>
                        <p className="text-sm font-medium text-foreground">
                          {i + 1}. {l.title}
                        </p>
                        <p className="text-xs text-muted-foreground">{l.durationMinutes} min</p>
                      </div>
                    </div>
                    <Button
                      variant="ghost"
                      size="sm"
                      onClick={() => handleMarkComplete(l.id)}
                      disabled={!isAuthenticated}
                    >
                      {isCompleted ? "Review" : "Start"}
                    </Button>
                  </li>
                );
              })}
            </ul>
          </section>

          <aside className="space-y-3 rounded-lg border border-border bg-muted/30 p-5">
            <h3 className="text-sm font-semibold text-foreground">Course actions</h3>
            <p className="text-xs text-muted-foreground">
              {course.lessons.length} lessons · {course.durationMinutes} min · {course.difficulty}
            </p>
            <p className="text-xs text-muted-foreground">
              Certificates are issued upon completion of all lessons and the final assessment.
            </p>
          </aside>
        </div>
      </article>
    </SiteShell>
  );
}