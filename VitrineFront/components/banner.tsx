import Link from "next/link"
import { Button } from "@/components/ui/button"

export default function Banner() {
  return (
    <section className="relative bg-gradient-to-r from-primary/10 to-primary/5 py-20 px-4 md:px-6 lg:px-8">
      <div className="container mx-auto">
        <div className="grid grid-cols-1 md:grid-cols-2 gap-12 items-center">
          <div className="text-center md:text-left">
            <h1 className="text-4xl md:text-5xl lg:text-6xl font-bold mb-6">Crie sua loja online em minutos</h1>
            <p className="text-xl text-gray-600 mb-8 max-w-lg mx-auto md:mx-0">
              Plataforma completa para você vender seus produtos online, sem complicações e com total controle.
            </p>
            <div className="flex flex-col sm:flex-row gap-4 justify-center md:justify-start">
              <Link href="/cadastro">
                <Button size="lg" className="px-8">
                  Cadastrar
                </Button>
              </Link>
              <Link href="/login">
                <Button variant="outline" size="lg" className="px-8">
                  Entrar
                </Button>
              </Link>
            </div>
          </div>
          <div className="hidden md:block">
            <div className="relative">
              <div className="absolute -top-6 -left-6 w-24 h-24 bg-primary/10 rounded-lg"></div>
              <div className="absolute -bottom-6 -right-6 w-24 h-24 bg-primary/20 rounded-lg"></div>
              <div className="relative bg-white p-6 rounded-lg shadow-lg">
                <img
                  src="/placeholder.svg?height=400&width=500"
                  alt="Plataforma de loja online"
                  className="rounded-md w-full h-auto"
                />
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Decorative elements */}
      <div className="absolute top-1/2 right-0 w-24 h-24 bg-primary/5 rounded-full -translate-y-1/2 hidden lg:block"></div>
      <div className="absolute bottom-0 left-1/4 w-12 h-12 bg-primary/10 rounded-full hidden lg:block"></div>
    </section>
  )
}
