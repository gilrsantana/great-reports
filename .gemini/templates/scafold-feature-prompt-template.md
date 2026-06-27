Por favor, invoque o subagente dotnet-boilerplate-architect para implementar uma nova entidade completa no projeto chamada {EntityName}.
Instruções para o agente:
1. Para começar, leia a nossa skill de roteamento em .gemini/skills/00-scaffold-feature-playbook/SKILL.md .
2. Siga rigorosamente a ordem sequencial das fases descritas no checklist do playbook (da Fase 1 à Fase 6).
3. Use as regras de negócio em .gemini/rules/ e as skills em .gemini/skills/ para gerar os arquivos necessários:
    • A classe de domínio {EntityName} herdando de BaseEntity com UUID v7.
    • Os casos de uso de aplicação (Commands, Queries, Handlers e Responses) para gerenciar o {EntityName}.
    • O mapeamento Fluent API da tabela {EntityNamePlural} e a classe {EntityName}Repository herdando de BaseEntityRepository (lembre-se: sem implementar exclusão física!).
    • Os registros na injeção de dependência das camadas.
    • O controller {EntityNamePlural}Controller com anotações OpenAPI/Scalar.
    • Os testes unitários com Moq.
4. Ao concluir, execute dotnet build e confirme se todos os testes passaram e se o código compila corretamente.
