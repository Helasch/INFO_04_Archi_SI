using System.Linq.Expressions;
using Moq;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.Entities;
using UniversiteDomain.UseCases.ParcoursUseCases.Add;
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
    
     [Test]
    public async Task AddEtudiantDansParcours()
    {
        long idEtudiant = 1;
        long idParcours = 3;
        Etudiant etudiant= new Etudiant { Id = 1, NumEtud = "1", Nom = "nom1", Prenom = "prenom1", Email = "1" };
        Parcours parcours = new Parcours{Id=3, NomParcours = "Ue 3", AnneeFormation = 1};
        
        // On initialise une fausse datasource qui va simuler un EtudiantRepository
        var mockEtudiant = new Mock<IEtudiantRepository>();
        var mockParcours = new Mock<IParcoursRepository>();
        List<Etudiant> etudiants = new List<Etudiant>();
        etudiants.Add(new Etudiant{Id=1});
        mockEtudiant
            .Setup(repo=>repo.FindByConditionAsync(e=>e.Id.Equals(idEtudiant)))
            .ReturnsAsync(etudiants);

        List<Parcours> parcourses = new List<Parcours>();
        parcourses.Add(parcours);
        
        List<Parcours> parcoursFinaux = new List<Parcours>();
        Parcours parcoursFinal = new Parcours{Id=3, NomParcours = "Ue 3", AnneeFormation = 1};
        parcoursFinal.Inscrits.Add(etudiant);
        parcoursFinaux.Add(parcours);
        
        mockParcours
            .Setup(repo=>repo.FindByConditionAsync(e=>e.Id.Equals(idParcours)))
            .ReturnsAsync(parcourses);
        mockParcours
            .Setup(repo => repo.AddEtudiantAsync(idParcours, idEtudiant))
            .ReturnsAsync(parcoursFinal);
        // Création du use case en utilisant le mock comme datasource
        AddEtudiantDansParcours useCase=new AddEtudiantDansParcours(mockEtudiant.Object, mockParcours.Object);
        
        // Appel du use case
        var parcoursTest=await useCase.ExecuteAsync(idParcours, idEtudiant);
        // Vérification du résultat
        Assert.That(parcoursTest.Id, Is.EqualTo(parcoursFinal.Id));
        Assert.That(parcoursTest.Inscrits, Is.Not.Null);
        Assert.That(parcoursTest.Inscrits.Count, Is.EqualTo(1));
        Assert.That(parcoursTest.Inscrits[0].Id, Is.EqualTo(idEtudiant));
    }
}