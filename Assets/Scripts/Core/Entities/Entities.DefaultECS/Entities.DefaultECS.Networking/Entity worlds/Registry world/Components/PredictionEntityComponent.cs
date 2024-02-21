 using DefaultEcs;

 namespace HereticalSolutions.Entities
 {
    [Component("Prediction world")]
    [WorldIdentityComponent]
    public struct PredictionEntityComponent
     {
         /// <summary>
         /// The prediction entity.
         /// </summary>
         public Entity PredictionEntity;

         /// <summary>
         /// The ID of the prototype.
         /// </summary>
         public string PrototypeID;
     }
 }