using System.Linq.Expressions;
using Moq;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.ParcoursUseCases.Create;

namespace UniversiteDomainUnitTest;

public class ParcoursUnitTests
{
    [SetUp]
    public void Setup()
    {
        
    }

    [Test]
    public async Task CreateParcoursUseCase()
    {
        long id = 1;
        String nomParcours = "TestParcours";
        int anneeFormation = 2022;
        
        //Création du parcours
        Parcours parcoursSansId = new Parcours{NomParcours = nomParcours, AnneeFormation = anneeFormation};
        
        //Création du mock
        var mock = new Mock<IParcoursRepository>();
        
        var reponseFindByCondition = new List<Parcours>();
        
        mock.Setup(repo => repo.FindByConditionAsync(It.IsAny<Expression<Func<Parcours, bool>>>())).ReturnsAsync(reponseFindByCondition);
        
        Parcours parcoursCree = new  Parcours{Id = id,NomParcours = nomParcours, AnneeFormation = anneeFormation};
        mock.Setup(repoParcours => repoParcours.CreateAsync(parcoursSansId)).ReturnsAsync(parcoursCree);
        
        var fauxParcoursRepository = mock.Object;
        
        CreateParcoursUseCase useCase = new CreateParcoursUseCase(fauxParcoursRepository);
        //Appel du use case
        var parcoursTeste = await useCase.ExecuteAsync(parcoursSansId);
        
        //Assert
        Assert.That(parcoursTeste.Id, Is.EqualTo(parcoursCree.Id));
        Assert.That(parcoursTeste.NomParcours, Is.EqualTo(parcoursCree.NomParcours));
        Assert.That(parcoursTeste.AnneeFormation, Is.EqualTo(parcoursCree.AnneeFormation));
    }
}