"use client"

import type React from "react"
import { useState } from "react"
import { useRouter } from "next/navigation"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select"
import { Checkbox } from "@/components/ui/checkbox"
import { motion } from "framer-motion"
import { useToast } from "@/hooks/use-toast"
import Navbar from "@/components/navbar"

export default function MultiStepForm() {
  const router = useRouter()
  const { toast } = useToast()
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [step, setStep] = useState(1)
  const [formData, setFormData] = useState({
    cpf: "",
    nome: "",
    dataNascimento: "",
    cnpj: "",
    nomeLoja: "",
    categoriaVenda: "",
    email: "",
    senha: "",
    confirmarSenha: "",
    // lembrarDeMim: false,
  })

  const resetForm = () => {
    setFormData({
      cpf: "",
      nome: "",
      dataNascimento: "",
      cnpj: "",
      nomeLoja: "",
      categoriaVenda: "",
      email: "",
      senha: "",
      confirmarSenha: "",
      // lembrarDeMim: false,
    })
    setStep(1)
  }

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target
    setFormData((prev) => ({ ...prev, [name]: value }))
  }

  const handleSelectChange = (value: string, field: string) => {
    setFormData((prev) => ({ ...prev, [field]: value }))
  }

  // const handleCheckboxChange = (checked: boolean) => {
  //   setFormData((prev) => ({ ...prev, lembrarDeMim: checked }))
  // }

  const handleNext = () => {
    setStep((prevStep) => prevStep + 1)
  }

  const handleBack = () => {
    setStep((prevStep) => prevStep - 1)
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setIsSubmitting(true)

    try {
      const response = await fetch("https://f940-177-131-178-100.ngrok-free.app/api/Account/register", {
        method: "POST",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify({
          Name: formData.nome,
          Email: formData.email,
          Password: formData.senha,
          ConfirmPassword: formData.confirmarSenha,
          Cpf: formData.cpf,
          DataNascimento: formData.dataNascimento,
          Cnpj: formData.cnpj,
          NomeLoja: formData.nomeLoja,
          CategoriaVenda: formData.categoriaVenda,
        })
      })

      if (!response.ok) {
        const errorText = await response.text()
        toast({
          title: "Erro no cadastro",
          description: errorText,
          variant: "destructive",
          duration: 5000
        })
        setIsSubmitting(false)
        return
      }

      // Sucesso
      toast({
        title: "Cadastro realizado com sucesso!",
        description: "Você será redirecionado para a página de login.",
        duration: 5000
      })

      resetForm() // Limpar os campos
      setTimeout(() => {
        router.push("/login") // Redirecionar
      }, 0) // delay após cadastro

    } catch (error) {
      console.error("Erro ao cadastrar:", error)
      toast({
        title: "Erro inesperado",
        description: "Não foi possível realizar o cadastro.",
        variant: "destructive",
        duration: 5000
      })
    } finally {
      setIsSubmitting(false)
    }
  }


  const getStepTitle = () => {
    switch (step) {
      case 1:
        return "Cadastro de Usuário"
      case 2:
        return "Informações da Loja"
      case 3:
        return "Credenciais de Acesso"
      default:
        return "Cadastro"
    }
  }

  const getStepDescription = () => {
    switch (step) {
      case 1:
        return "Preencha suas informações pessoais"
      case 2:
        return "Informe os dados da sua loja"
      case 3:
        return "Configure suas credenciais de acesso"
      default:
        return ""
    }
  }

  return (
    <div className="min-h-screen flex flex-col bg-gray-50">
      <Navbar />
      <div className="flex-1 flex items-center justify-center p-4">
        <Card className="w-full max-w-md overflow-hidden">
          <CardHeader className="bg-white">
            <CardTitle className="text-2xl">{getStepTitle()}</CardTitle>
            <CardDescription>{getStepDescription()}</CardDescription>
          </CardHeader>
          <CardContent className="relative overflow-hidden p-0">
            <div
              className="flex transition-transform duration-500"
              style={{ transform: `translateX(-${(step - 1) * 100}%)` }}
            >
              {/* Etapa 1: Informações Pessoais */}
              <div className="w-full flex-shrink-0 p-6">
                <form className="space-y-4">
                  <div className="space-y-2">
                    <Label htmlFor="cpf">CPF</Label>
                    <Input
                      id="cpf"
                      name="cpf"
                      placeholder="Digite seu CPF"
                      value={formData.cpf}
                      onChange={handleChange}
                      required
                    />
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="nome">Nome Completo</Label>
                    <Input
                      id="nome"
                      name="nome"
                      placeholder="Digite seu nome completo"
                      value={formData.nome}
                      onChange={handleChange}
                      required
                    />
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="dataNascimento">Data de Nascimento</Label>
                    <Input
                      id="dataNascimento"
                      name="dataNascimento"
                      type="date"
                      value={formData.dataNascimento}
                      onChange={handleChange}
                      required
                    />
                  </div>
                </form>
              </div>

              {/* Etapa 2: Informações da Loja */}
              <div className="w-full flex-shrink-0 p-6">
                <form className="space-y-4">
                  <div className="space-y-2">
                    <Label htmlFor="cnpj">CNPJ (opcional)</Label>
                    <Input
                      id="cnpj"
                      name="cnpj"
                      placeholder="Digite o CNPJ da sua loja"
                      value={formData.cnpj}
                      onChange={handleChange}
                    />
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="nomeLoja">Nome da Loja</Label>
                    <Input
                      id="nomeLoja"
                      name="nomeLoja"
                      placeholder="Digite o nome da sua loja"
                      value={formData.nomeLoja}
                      onChange={handleChange}
                      required
                    />
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="categoriaVenda">Categoria de Venda</Label>
                    <Select
                      onValueChange={(value) => handleSelectChange(value, "categoriaVenda")}
                      value={formData.categoriaVenda}
                    >
                      <SelectTrigger id="categoriaVenda">
                        <SelectValue placeholder="Selecione a categoria" />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="roupas">Roupas e Acessórios</SelectItem>
                        <SelectItem value="eletronicos">Eletrônicos</SelectItem>
                        <SelectItem value="alimentos">Alimentos</SelectItem>
                        <SelectItem value="beleza">Beleza e Cuidados Pessoais</SelectItem>
                        <SelectItem value="casa">Casa e Decoração</SelectItem>
                        <SelectItem value="esportes">Esportes e Lazer</SelectItem>
                        <SelectItem value="outros">Outros</SelectItem>
                      </SelectContent>
                    </Select>
                  </div>
                </form>
              </div>

              {/* Etapa 3: Informações de Conta */}
              <div className="w-full flex-shrink-0 p-6">
                <form className="space-y-4" onSubmit={handleSubmit}>
                  <div className="space-y-2">
                    <Label htmlFor="email">Email</Label>
                    <Input
                      id="email"
                      name="email"
                      type="email"
                      placeholder="seu.email@exemplo.com"
                      value={formData.email}
                      onChange={handleChange}
                      required
                    />
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="senha">Senha</Label>
                    <Input
                      id="senha"
                      name="senha"
                      type="password"
                      placeholder="Digite sua senha"
                      value={formData.senha}
                      onChange={handleChange}
                      required
                    />
                  </div>
                  <div className="space-y-2">
                    <Label htmlFor="confirmarSenha">Confirmar Senha</Label>
                    <Input
                      id="confirmarSenha"
                      name="confirmarSenha"
                      type="password"
                      placeholder="Confirme sua senha"
                      value={formData.confirmarSenha}
                      onChange={handleChange}
                      required
                    />
                  </div>
                  {/* <div className="flex items-center space-x-2">
                    <Checkbox
                      id="lembrarDeMim"
                      checked={formData.lembrarDeMim}
                      onCheckedChange={handleCheckboxChange}
                    />
                    <Label htmlFor="lembrarDeMim" className="text-sm font-normal">
                      Lembrar-se de mim
                    </Label>
                  </div> */}
                </form>
              </div>
            </div>
          </CardContent>

          <CardFooter className="flex justify-between border-t p-6">
            {step > 1 && (
              <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} transition={{ duration: 0.3 }}>
                <Button variant="outline" onClick={handleBack} disabled={isSubmitting}>
                  Voltar
                </Button>
              </motion.div>
            )}

            <motion.div
              key={step}
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              transition={{ duration: 0.3 }}
              className={step === 1 ? "ml-auto" : ""}
            >
              {step < 3 ? (
                <Button onClick={handleNext} disabled={isSubmitting}>
                  Prosseguir
                </Button>
              ) : (
                <Button type="submit" onClick={handleSubmit} disabled={isSubmitting}>
                  {isSubmitting ? "Cadastrando..." : "Cadastrar"}
                </Button>
              )}
            </motion.div>
          </CardFooter>
        </Card>
      </div>
    </div>
  )
}
