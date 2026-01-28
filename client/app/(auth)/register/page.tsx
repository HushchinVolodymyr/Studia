
import { RegisterForm } from "@/components/auth/register-form";
import { ThemeToggle } from '@/components/theme-toggle';

const RegisterPage = () => {
    return (
        <div className="bg-background flex min-h-svh flex-col items-center justify-center gap-6 p-6 md:p-10">
            <div className="absolute top-4 right-4">
                <ThemeToggle />
            </div>
            <div className="w-full max-w-sm">
                <RegisterForm />
            </div>
        </div>
    );
};

export default RegisterPage;